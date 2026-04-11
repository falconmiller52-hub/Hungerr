using System.Collections;
using System.Collections.Generic;
using Runtime.Features.Inventory;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    public InventoryWithCells inventory; // Твоя логика
    public GameObject slotPrefab;
    public float cellSize = 0.1f; // Размер одной ячейки в метрах

    void Start() {
        // Создаем визуальные ячейки
        for (int y = 0; y < inventory.Height; y++) {
            for (int x = 0; x < inventory.Width; x++) {
                Vector3 pos = new Vector3(x * cellSize, -y * cellSize, 0);
                Instantiate(slotPrefab, transform.position + pos, transform.rotation, transform);
            }
        }
    }
}
