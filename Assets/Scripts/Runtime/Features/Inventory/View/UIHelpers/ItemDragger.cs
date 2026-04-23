using FMODUnity;
using Runtime.Common.Services.Audio;
using Runtime.Features.Sounds;
using Runtime.Features.Inventory;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Inventory.View.UIHelpers
{
	/// <summary>
	/// хендлит то если игрок мышью попытается взять предмет при открытом инвентаре
	/// мб должен лежать как компонент вместе с инвентарем
	/// </summary>
	public class ItemDragger : MonoBehaviour
	{
		[Header("References")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Inventory3DView _view;
        [SerializeField] private LayerMask _gridLayer;
        [SerializeField] private EventReference _moveItemToNewSlotSound;

        [Header("Settings")]
        [SerializeField] private bool _isDraggedItemTransparent = true;
        
        private Inventory3DView _viewChestCompat;
        private InventoryItem _selectedItem;
        private (Vector2Int, Inventory3DView) _originalPositionInInventory;
        private InventoryItemView _ghostItem;
        private IAudioService _audioService;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public void OpenChest(Inventory3DView view)
        {
            _viewChestCompat = view;
        }

        public void CloseChest()
        {
            _viewChestCompat = null;
        }
        
        void Update()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, _gridLayer))
            {
                // Переводим точку хита в локальные координаты сетки
                Vector3 localPos = _view.GridAnchor.InverseTransformPoint(hit.point);
                Vector2Int hoveredCoords = LocalToGrid(localPos);

                if (_selectedItem == null)
                {
                    HandleSelection(hoveredCoords, hit.point);
                }
                else
                {
                    HandlePlacement(hoveredCoords);
                }
            }
            else if (Input.GetMouseButtonDown(1)) // Отмена на ПКМ
            {
                CancelDragging();
            }
        }

        private void HandleSelection(Vector2Int coords, Vector3 point)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Try taking from player inventory first
                var slot = _view.Model.GetSlot(coords);
                bool takenFromPlayer = slot != null && !slot.IsEmpty;
                if (takenFromPlayer)
                {
                    _selectedItem = slot.Item;
                    _originalPositionInInventory.Item1 = FindTopLeftOfItem(_selectedItem, coords, _view);
                    _originalPositionInInventory.Item2 = _view;
                    RemoveItemFromLogic(_selectedItem, _originalPositionInInventory.Item1, _originalPositionInInventory.Item2);
                    PrepareGhost(_selectedItem);
                    return;
                }
                // If nothing in player's slot, try chest if available
                if (_viewChestCompat != null && _viewChestCompat.Model != null)
                {
                    Vector3 localPos = _viewChestCompat.GridAnchor.InverseTransformPoint(point);
                    coords = LocalToGrid(localPos);
                    
                    var chestSlot = _viewChestCompat.Model.GetSlot(coords);
                    if (chestSlot != null && !chestSlot.IsEmpty)
                    {
                        _selectedItem = chestSlot.Item;
                        _originalPositionInInventory.Item1 = FindTopLeftOfItem(_selectedItem, coords, _viewChestCompat);
                        _originalPositionInInventory.Item2 = _viewChestCompat;
                        // Remove from chest logic (best effort)
                        RemoveItemFromLogic(_selectedItem, _originalPositionInInventory.Item1, _originalPositionInInventory.Item2);
                        PrepareGhost(_selectedItem);
                    }
                }
            }
        }

        private void HandlePlacement(Vector2Int coords)
        {
            // Обновляем позицию "призрака" для предпросмотра
            Vector3 targetPos = _view.GridToLocal(coords, _selectedItem.Data.Width, _selectedItem.Data.Height);
            _ghostItem.UpdateVisualPosition(targetPos, false);

            bool canPlace = _view.Model.CanPlaceItem(_selectedItem, coords);
            UpdateGhostColor(canPlace);

            if (Input.GetMouseButtonDown(0))
            {
                if (canPlace)
                {
                    // Подтверждаем установку в текущий Inventory (Player)
                    _view.Model.AddItem(_selectedItem, coords);
                    ClearSelection();
                }
                else if (_viewChestCompat != null && _viewChestCompat.Model != null &&
                         _viewChestCompat.Model.CanPlaceItem(_selectedItem, coords))
                {
                    // Попытка разместить в сундук
                    _viewChestCompat.Model.AddItem(_selectedItem, coords);
                    ClearSelection();
                }
                else
                {
                    // Возвращаем на место, если нельзя поставить
                    var currentView = _originalPositionInInventory.Item2;
                    currentView.Model.AddItem(_selectedItem, _originalPositionInInventory.Item1);
                    ClearSelection();
                }
            }
        }

        private Vector2Int LocalToGrid(Vector3 localPos)
        {
            int x = Mathf.FloorToInt(localPos.x / _view.CellSize);
            int y = Mathf.FloorToInt(-localPos.z / _view.CellSize);
            
            // Ограничиваем координатами сетки
            x = Mathf.Clamp(x, 0, _view.Model.Width - 1);
            y = Mathf.Clamp(y, 0, _view.Model.Height - 1);
            
            return new Vector2Int(x, y);
        }

        private void PrepareGhost(InventoryItem item)
        {
            // Можно просто заспавнить тот же префаб
            GameObject ghostObj = Instantiate(item.Data.PrefabForInventory, _view.ItemsContainer);
            ghostObj.transform.localScale = new Vector3(item.Data.Width, 1, item.Data.Height);
            _ghostItem = ghostObj.GetComponent<InventoryItemView>();
            
            // Делаем полупрозрачным (нужен шейдер с поддержкой прозрачности)

            if (!_isDraggedItemTransparent)
                return;
            
            var renderers = ghostObj.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                foreach (var mat in r.materials)
                {
                    Color c = mat.color;
                    c.a = 0.5f;
                    mat.color = c;
                }
            }
        }

        private void UpdateGhostColor(bool canPlace)
        {
            // Опционально: красим в красный если нельзя поставить
            // _ghostItem.SetHighlight(canPlace ? Color.green : Color.red);
        }

        private void RemoveItemFromLogic(InventoryItem item, Vector2Int topLeft, Inventory3DView view)
        {
            if (view == null)
                return;
            
            for (int y = 0; y < item.Data.Height; y++)
            {
                for (int x = 0; x < item.Data.Width; x++)
                {
                    var slot = view.Model.GetSlot(new Vector2Int(topLeft.x + x, topLeft.y + y));
                    slot.Item = null;
                    slot.Id = -1;
                }
            }
            // Вызываем событие обновления визуала, чтобы основной меш исчез
            // (Так как мы создали ghost, а основной SyncVisuals его удалит из-за отсутствия в логике)
            view.Invoke("SyncVisuals", 0); 
        }

        private Vector2Int FindTopLeftOfItem(InventoryItem item, Vector2Int startCoords, Inventory3DView view)
        {
            if (view == null)
                return Vector2Int.zero;
            // Поиск реального начала предмета по его ID
            // Можно реализовать через цикл по соседям или хранить ID в слоте
            // Для упрощения возьмем логику из твоего GetUniqueItemsWithTopLeft
            int targetId = view.Model.GetSlot(startCoords).Id;
            Vector2Int min = startCoords;

            foreach (var kvp in view.Model.Slots)
            {
                if (kvp.Value.Id == targetId)
                {
                    if (kvp.Key.x < min.x) min.x = kvp.Key.x;
                    if (kvp.Key.y < min.y) min.y = kvp.Key.y;
                }
            }
            return min;
        }

        private void ClearSelection()
        {
            if (_ghostItem != null) Destroy(_ghostItem.gameObject);
            _selectedItem = null;
            _ghostItem = null;
            // Обновляем визуал, чтобы меш встал на место
            _view.Invoke("SyncVisuals", 0);
            _viewChestCompat?.Invoke("SyncVisuals", 0);
        }

        private void CancelDragging()
        {
            if (_selectedItem != null)
            {
                var currentView = _originalPositionInInventory.Item2;
                currentView.Model.AddItem(_selectedItem, _originalPositionInInventory.Item1);
                ClearSelection();
            }
        }
	}
}
