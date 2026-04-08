using System;
using UnityEngine;
using UnityEngine.UI;
using Runtime.Features.Inventory;
using System.Collections.Generic;

public class InventoryDisplayUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float cellSize = 50f;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject itemPrefab;

    [Header("Containers")]
    [SerializeField] private Transform slotContainer;
    [SerializeField] private Transform itemContainer;


    private Dictionary<int, InventoryItemUI> spawnedItems = new Dictionary<int, InventoryItemUI>();

    private InventoryWithCells inventory;
    [SerializeField] private PlayerInventory playerInventory;
    
    
    private void Start()
    {
        Initialize(playerInventory.GetInventory());
    }

    public void Initialize(InventoryWithCells inv)
    {
        this.inventory = inv;

        playerInventory.OnInventoryChanged += RefreshItems; // отписку
        
        DrawGrid();
    }

    private void DrawGrid()
    {
        // Очистка контейнера
        foreach (Transform child in slotContainer) Destroy(child.gameObject);

        // Создаем визуальные клетки (просто картинки-квадратики)
        for (int y = 0; y < inventory.Height; y++)
        {
            for (int x = 0; x < inventory.Width; x++)
            {
                var slotGo = Instantiate(slotPrefab, slotContainer);
                var rect = slotGo.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(cellSize, cellSize);
                rect.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);
            }
        }
    }

    public void RefreshItems()
    {
        // В продакшене лучше не удалять всё, а сравнивать список, 
        // но для начала удалим старые иконки
        foreach (var itemUI in spawnedItems.Values) Destroy(itemUI.gameObject);
        spawnedItems.Clear();

        // Проходим по слотам инвентаря
        foreach (var pair in inventory.Slots)
        {
            var slot = pair.Value;
            if (!slot.IsEmpty)
            {
                // Если предмет многослотовый, он записан в нескольких ячейках.
                // Чтобы не спавнить иконку дважды, используем Id.
                if (spawnedItems.ContainsKey(slot.Id)) 
                    continue;

                CreateItemUI(slot);
            }
        }
    }

    
    // обязательно контейнеры Slots and Items должны быть с пивотом (0, 1) и якоря и мин и макс на левый верхний угол (0, 1)
    private void CreateItemUI(InventorySlot slot)
    {
        var itemGo = Instantiate(itemPrefab, itemContainer);
        var itemUI = itemGo.GetComponent<InventoryItemUI>();
        
        itemUI.Setup(slot.item._data.icon, slot.item._data.width, slot.item._data.height, cellSize);
        
        // Позиция: нужно найти "верхнюю левую" точку предмета.
        // В твоем коде логичнее хранить стартовую позицию в самом InventoryItem или передавать её.
        // Если предмет занимает (2,2), то отрисовываем его от текущих координат слота.
        itemUI.SetPosition(slot.position, cellSize);
        
        spawnedItems.Add(slot.Id, itemUI);
    }
}