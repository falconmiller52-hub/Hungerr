using System;
using Runtime.Common.Services.Pause;
using UnityEngine;
using Runtime.Features.Inventory.View.New;
using Zenject;

namespace Runtime.Features.Inventory.Control
{
    public class InventoryInputController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Inventory3DView _view;
        [SerializeField] private LayerMask _gridLayer;
        [SerializeField] private PlayerInventory _playerInventory;
        
        private InventoryItem _selectedItem;
        private Vector2Int _originalPosition;
        private InventoryItemView _ghostItem;
        private IPauseController _pauseController;

        [Inject]
        public void Construct(IPauseController pauseController)
        {
            _pauseController = pauseController;
        }
        
        private void Start()
        {
            
            _playerInventory.OnInventoryOpenStateChanged += InventoryChangeVisualState;
        }
    
        private void OnDestroy()
        {
            _playerInventory.OnInventoryOpenStateChanged -= InventoryChangeVisualState;
        }
    
        private void InventoryChangeVisualState(bool active)
        {
            if (active)
            {
                Debug.Log("Enable");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _pauseController.PerformStop();
            }
            else
            {
                Debug.Log("Disable");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _pauseController.PerformResume();
            }
        }

        void Update()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, _gridLayer))
            {
                // Переводим точку хита в локальные координаты сетки
                Vector3 localPos = _view.GridAchor.InverseTransformPoint(hit.point);
                Vector2Int hoveredCoords = LocalToGrid(localPos);

                if (_selectedItem == null)
                {
                    HandleSelection(hoveredCoords);
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

        private void HandleSelection(Vector2Int coords)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var slot = _view._model.GetSlot(coords);
                if (slot != null && !slot.IsEmpty)
                {
                    _selectedItem = slot.item;
                    _originalPosition = FindTopLeftOfItem(_selectedItem, coords);

                    // Удаляем предмет из логики временно, чтобы он не мешал сам себе при проверке "CanPlace"
                    RemoveItemFromLogic(_selectedItem, _originalPosition);
                    
                    // Создаем призрака или берем существующий визуал
                    PrepareGhost(_selectedItem);
                }
            }
        }

        private void HandlePlacement(Vector2Int coords)
        {
            // Обновляем позицию "призрака" для предпросмотра
            Vector3 targetPos = _view.GridToLocal(coords, _selectedItem._data.width, _selectedItem._data.height);
            _ghostItem.UpdateVisualPosition(targetPos, false);

            bool canPlace = _view._model.CanPlaceItem(_selectedItem, coords);
            UpdateGhostColor(canPlace);

            if (Input.GetMouseButtonDown(0))
            {
                if (canPlace)
                {
                    // Подтверждаем установку
                    _view._model.AddItem(_selectedItem, coords);
                    ClearSelection();
                }
                else
                {
                    // Возвращаем на место, если нельзя поставить
                    _view._model.AddItem(_selectedItem, _originalPosition);
                    ClearSelection();
                }
            }
        }

        private Vector2Int LocalToGrid(Vector3 localPos)
        {
            int x = Mathf.FloorToInt(localPos.x / _view.CellSize);
            int y = Mathf.FloorToInt(-localPos.z / _view.CellSize);
            
            // Ограничиваем координатами сетки
            x = Mathf.Clamp(x, 0, _view._model.Width - 1);
            y = Mathf.Clamp(y, 0, _view._model.Height - 1);
            
            return new Vector2Int(x, y);
        }

        private void PrepareGhost(InventoryItem item)
        {
            // Можно просто заспавнить тот же префаб
            GameObject ghostObj = Instantiate(item._data.PrefabForInventory, _view.ItemsContainer);
            _ghostItem = ghostObj.GetComponent<InventoryItemView>();
            
            // Делаем полупрозрачным (нужен шейдер с поддержкой прозрачности)
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

        private void RemoveItemFromLogic(InventoryItem item, Vector2Int topLeft)
        {
            for (int y = 0; y < item._data.height; y++)
            {
                for (int x = 0; x < item._data.width; x++)
                {
                    var slot = _view._model.GetSlot(new Vector2Int(topLeft.x + x, topLeft.y + y));
                    slot.item = null;
                    slot.Id = -1;
                }
            }
            // Вызываем событие обновления визуала, чтобы основной меш исчез
            // (Так как мы создали ghost, а основной SyncVisuals его удалит из-за отсутствия в логике)
            _view.Invoke("SyncVisuals", 0); 
        }

        private Vector2Int FindTopLeftOfItem(InventoryItem item, Vector2Int startCoords)
        {
            // Поиск реального начала предмета по его ID
            // Можно реализовать через цикл по соседям или хранить ID в слоте
            // Для упрощения возьмем логику из твоего GetUniqueItemsWithTopLeft
            int targetId = _view._model.GetSlot(startCoords).Id;
            Vector2Int min = startCoords;

            foreach (var kvp in _view._model.Slots)
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
        }

        private void CancelDragging()
        {
            if (_selectedItem != null)
            {
                _view._model.AddItem(_selectedItem, _originalPosition);
                ClearSelection();
            }
        }
    }
}