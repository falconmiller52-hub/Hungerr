using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Runtime.Features.Inventory;

public class Inventory3DView : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 0.1f;
    public Transform itemsContainer;
    public InventoryItemView itemPrefab;

    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    
    public InventoryWithCells Logic { get; private set; }
    private readonly Dictionary<int, InventoryItemView> _spawnedItems = new Dictionary<int, InventoryItemView>();

    private void Start()
    {
        Logic = playerInventory.GetInventory();
        playerInventory.OnInventoryChanged += SyncVisuals;
        SyncVisuals();
    }

    private void SyncVisuals()
    {
        if (Logic == null) return;

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
            Vector3 targetPos = GridToLocal(topLeft, pair.Value.item._data.width, pair.Value.item._data.height);

            if (!_spawnedItems.ContainsKey(id))
            {
                var newItem = Instantiate(pair.Value.item._data.PrefabForInventory, itemsContainer).GetComponent<InventoryItemView>();
                newItem.Setup(itemData);
                newItem.UpdateVisualPosition(targetPos, true); // Мгновенно при спавне
                _spawnedItems.Add(id, newItem);
            }
            else
            {
                _spawnedItems[id].UpdateVisualPosition(targetPos); // Плавно через PrimeTween
            }
        }
    }

    private Dictionary<int, (Vector2Int pos, InventoryItem item)> GetUniqueItemsWithTopLeft()
    {
        var result = new Dictionary<int, (Vector2Int, InventoryItem)>();

        foreach (var kvp in Logic.Slots)
        {
            var slot = kvp.Value;
            if (slot.IsEmpty || slot.Id == -1) continue;

            if (!result.ContainsKey(slot.Id))
            {
                result.Add(slot.Id, (kvp.Key, slot.item));
            }
            else
            {
                // Ищем самый левый верхний угол (минимальные X и Y)
                var current = result[slot.Id];
                if (kvp.Key.x <= current.Item1.x && kvp.Key.y <= current.Item1.y)
                {
                    result[slot.Id] = (kvp.Key, slot.item);
                }
            }
        }
        return result;
    }

    public Vector3 GridToLocal(Vector2Int gridPos, float width, float height)
    {
        // Помним: в инвентаре Y идет вниз, в 3D это -Z
        return new Vector3((gridPos.x + width / 2) * cellSize, 0, -(gridPos.y + height / 2) * cellSize);
    }
    
    private void OnDrawGizmos()
    {
        // Если логика еще не инициализирована (в эдите), используем тестовые значения
        // или пытаемся достать их из ScriptableObject/полей
        int drawWidth = (Logic != null) ? Logic.Width : 10; 
        int drawHeight = (Logic != null) ? Logic.Height : 10;

        Gizmos.matrix = transform.localToWorldMatrix; // Учитываем поворот и позицию чемодана

        for (int y = 0; y < drawHeight; y++)
        {
            for (int x = 0; x < drawWidth; x++)
            {
                // Получаем центр ячейки для отрисовки куба
                // Смещение на cellSize * 0.5f нужно, чтобы куб рисовался не из угла, а центрировался по сетке
                Vector3 cellCenter = new Vector3(
                    x * cellSize + (cellSize * 0.5f), 
                    0, 
                    -y * cellSize - (cellSize * 0.5f)
                );

                // Чередуем цвета для шахматного эффекта (удобнее для глаз)
                Gizmos.color = (x + y) % 2 == 0 ? new Color(0, 1, 1, 0.2f) : new Color(0, 1, 1, 0.1f);
            
                // Рисуем заполненный квадрат ячейки
                Gizmos.DrawCube(cellCenter, new Vector3(cellSize * 0.95f, 0.01f, cellSize * 0.95f));

                // Рисуем контур ячейки
                Gizmos.color = new Color(0, 1, 1, 0.5f);
                Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, 0, cellSize));
            }
        }

        // Рисуем яркую рамку вокруг всего инвентаря
        Gizmos.color = Color.yellow;
        Vector3 totalCenter = new Vector3(
            (drawWidth * cellSize) * 0.5f, 
            0, 
            -(drawHeight * cellSize) * 0.5f
        );
        Gizmos.DrawWireCube(totalCenter, new Vector3(drawWidth * cellSize, 0.02f, drawHeight * cellSize));
    }
}