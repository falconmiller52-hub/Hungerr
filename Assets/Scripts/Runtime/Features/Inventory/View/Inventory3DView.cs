using System.Collections.Generic;
using System.Linq;
using Runtime.Features.Inventory;
using UnityEngine;

namespace Runtime.Features.Inventory.View
{
    public class Inventory3DView : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private float _cellSize = 0.1f;

        [Header("References")]
        [SerializeField] private PlayerInventory _playerInventory;
        // Optional chest model bound at runtime to enable dual-inventory UI via drag/drop
        [SerializeField] private InventoryWithCells _chestModel;
        [SerializeField] private GameObject _inventoryContainer;
        [SerializeField] private Transform _gridAnchor;
        [SerializeField] private Transform _itemsContainer;

        private InventoryWithCells _model;
        private readonly Dictionary<int, InventoryItemView> _spawnedItems = new Dictionary<int, InventoryItemView>();

        public InventoryWithCells Model => _model;
        // Chest model accessor for drag/drop helpers
        public InventoryWithCells ChestModel => _chestModel;
        // Bind a chest model to this view (to be called by Chest UI controller)
        public void BindChestInventory(InventoryWithCells chestModel)
        {
            _chestModel = chestModel;
        }
        public Transform GridAnchor => _gridAnchor;
        public float CellSize => _cellSize;
        public Transform ItemsContainer => _itemsContainer;
        
        private void Start()
        {
            _model = _playerInventory.GetInventory();
        
            _playerInventory.OnInventoryChanged += SyncVisuals;
            _playerInventory.OnInventoryOpenStateChanged += InventoryChangeVisualState;
        
            // предполагаем что из начально инвентарь у нас закрыт
            _inventoryContainer.SetActive(false);
            
            SyncVisuals();
        }
    
        private void OnDestroy()
        {
            _playerInventory.OnInventoryChanged -= SyncVisuals;
            _playerInventory.OnInventoryOpenStateChanged -= InventoryChangeVisualState;
        }
    
        private void InventoryChangeVisualState(bool active)
        {
            _inventoryContainer.SetActive(active);
        }

        private void SyncVisuals()
        {
            if (Model == null) 
                return;

            // Находим реальный Top-Left для каждого предмета
            var itemsInLogic = GetUniqueItemsWithTopLeft();

            // 1. Удаляем то, чего больше нет в данных
            var idsToRemove = _spawnedItems.Keys.Where(id => !itemsInLogic.ContainsKey(id)).ToList();
            foreach (var id in idsToRemove)
            {
                Destroy(_spawnedItems[id].gameObject);
                _spawnedItems.Remove(id);
            }

            // 2. Создаем или перемещаем
            foreach (var pair in itemsInLogic)
            {
                int id = pair.Key;
                Vector2Int topLeft = pair.Value.pos;
                InventoryItem itemData = pair.Value.item;
                Vector3 targetPos = GridToLocal(topLeft, pair.Value.item.Data.Width, pair.Value.item.Data.Height);

                if (!_spawnedItems.ContainsKey(id))
                {
                    var newItem = Instantiate(pair.Value.item.Data.PrefabForInventory, _itemsContainer).GetComponent<InventoryItemView>();
                    if (newItem == null)
                    {
                        Debug.LogError($"Inventory3DView::InventoryChangeVisualState() {newItem.Item} not found");
                        continue;
                    }
                    
                    newItem.Setup(itemData);
                    newItem.UpdateVisualPosition(targetPos, true); // Мгновенно при спавне
                    _spawnedItems.Add(id, newItem);
                }
                else
                {
                    _spawnedItems[id].Setup(itemData);
                    _spawnedItems[id].UpdateVisualPosition(targetPos); // Плавно через PrimeTween
                }
            }
        }

        private Dictionary<int, (Vector2Int pos, InventoryItem item)> GetUniqueItemsWithTopLeft()
        {
            var result = new Dictionary<int, (Vector2Int, InventoryItem)>();

            foreach (var kvp in Model.Slots)
            {
                var slot = kvp.Value;
                
                if (slot.IsEmpty || slot.Id == -1) 
                    continue;

                if (!result.ContainsKey(slot.Id))
                    result.Add(slot.Id, (kvp.Key, slot.Item));
                else
                {
                    // Ищем самый левый верхний угол (минимальные X и Y)
                    var current = result[slot.Id];
                    
                    if (kvp.Key.x <= current.Item1.x && kvp.Key.y <= current.Item1.y)
                        result[slot.Id] = (kvp.Key, slot.Item);
                    
                }
            }
            return result;
        }

        public Vector3 GridToLocal(Vector2Int gridPos, float width, float height)
        {
            // Помним: в инвентаре Y идет вниз, в 3D это -Z
            return new Vector3((gridPos.x + width / 2) * _cellSize, 0, -(gridPos.y + height / 2) * _cellSize);
        }
    
        private void OnDrawGizmos()
        {
            // Если логика еще не инициализирована (в эдите), используем тестовые значения
            // или пытаемся достать их из ScriptableObject/полей
            int drawWidth = (Model != null) ? Model.Width : 10; 
            int drawHeight = (Model != null) ? Model.Height : 10;

            Gizmos.matrix = _gridAnchor.localToWorldMatrix; // Учитываем поворот и позицию чемодана

            for (int y = 0; y < drawHeight; y++)
            {
                for (int x = 0; x < drawWidth; x++)
                {
                    // Получаем центр ячейки для отрисовки куба
                    // Смещение на cellSize * 0.5f нужно, чтобы куб рисовался не из угла, а центрировался по сетке
                    Vector3 cellCenter = new Vector3(
                        x * _cellSize + (_cellSize * 0.5f), 
                        0, 
                        -y * _cellSize - (_cellSize * 0.5f)
                    );

                    // Чередуем цвета для шахматного эффекта (удобнее для глаз)
                    Gizmos.color = (x + y) % 2 == 0 ? new Color(0, 1, 1, 0.2f) : new Color(0, 1, 1, 0.1f);
            
                    // Рисуем заполненный квадрат ячейки
                    Gizmos.DrawCube(cellCenter, new Vector3(_cellSize * 0.95f, 0.01f, _cellSize * 0.95f));

                    // Рисуем контур ячейки
                    Gizmos.color = new Color(0, 1, 1, 0.5f);
                    Gizmos.DrawWireCube(cellCenter, new Vector3(_cellSize, 0, _cellSize));
                }
            }

            // Рисуем яркую рамку вокруг всего инвентаря
            Gizmos.color = Color.yellow;
            Vector3 totalCenter = new Vector3(
                (drawWidth * _cellSize) * 0.5f, 
                0, 
                -(drawHeight * _cellSize) * 0.5f
            );
            Gizmos.DrawWireCube(totalCenter, new Vector3(drawWidth * _cellSize, 0.02f, drawHeight * _cellSize));
        }
    }
}
