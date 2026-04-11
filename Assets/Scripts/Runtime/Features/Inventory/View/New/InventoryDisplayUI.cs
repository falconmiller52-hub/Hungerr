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

    // 3D in-slot rendering
    [Header("3D In-Slot Rendering")]
    [SerializeField] private Transform worldRoot3D; // World-space root for spawned 3D models
    [SerializeField] private Vector3 cellWorldSize = new Vector3(0.25f, 0.25f, 0.25f); // world units per grid cell (x, y, z)


    // Spawned 3D item models (one per item footprint/group)
    private Dictionary<int, GameObject> spawned3DItems = new Dictionary<int, GameObject>();

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
        // Remove previously spawned 3D models for items occupying inventory slots
        foreach (var go in spawned3DItems.Values) Destroy(go);
        spawned3DItems.Clear();

        // Проходим по слотам инвентаря и создаем 3D модели, если они есть
        foreach (var pair in inventory.Slots)
        {
            var slot = pair.Value;
            if (!slot.IsEmpty)
            {
                if (spawned3DItems.ContainsKey(slot.Id))
                    continue;

                CreateItem3D(slot);
            }
        }
    }

    
    // обязательно контейнеры Slots and Items должны быть с пивотом (0, 1) и якоря и мин и макс на левый верхний угол (0, 1)
    private void CreateItem3D(InventorySlot slot)
    {
        var data = slot.item._data;
        // Choose 3D prefab for inventory display (prefer PrefabForInventory, fallback to worldPrefab)
        var prefab = data.PrefabForInventory != null ? data.PrefabForInventory : data.worldPrefab;
        if (prefab == null) return;

        var obj = Instantiate(prefab, worldRoot3D);
        // Position the 3D model to cover the footprint of the slot (and its multi-slot size)
        int w = data.width;
        int h = data.height;


        float centerX = (25 * w) + (slot.position.x * cellSize);
        float centerY = -(25 * h) - (slot.position.y * cellSize);
        obj.transform.localPosition = new Vector3(centerX, centerY, 0);

        // Scale the model to fit the footprint
        Vector3 scale = new Vector3(1, 1, 1)
        {
            x = w * 25,
            y = h * 25
        };
        obj.transform.localScale = scale;
        
        

        spawned3DItems.Add(slot.Id, obj);
    }
}
