// InventoryUIView.cs

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Runtime.Features.Inventory;
using UnityEngine.UI;

// This UI renders a tile-based inventory using slots (InventorySlotView) and a tile overlay
// for items that occupy multiple slots.
public class InventoryUIView : MonoBehaviour
{
    [SerializeField] private Transform _slotParent;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private int _slotSize = 100;
    
    [SerializeField] private int _gridWidth = 8;
    [SerializeField] private int _gridHeight = 4;

    private InventorySlotView[,] _slotViews;
    private void Start()
    {
        // var inventory = _playerInventory.GetInventory();
        //
        // var slots = inventory.Slots; // Dictionary<Vector2Int, InventorySlot>
        
        _playerInventory.OnInventoryChanged += Refresh;
        CreateSlots();
    }
    
    private void CreateSlots()
    {
        _slotViews = new InventorySlotView[_gridWidth, _gridHeight];
        
        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                var slotGO = Instantiate(_slotPrefab, _slotParent);
                var slotView = slotGO.GetComponent<InventorySlotView>();
                slotView.Initialize(new Vector2Int(x, y));
                _slotViews[x, y] = slotView;
            }
        }
    }
    
    public void Refresh()
    {
        var inventory = _playerInventory.GetInventory();
        
        // for (int y = 0; y < _gridHeight; y++)
        // {
        //     for (int x = 0; x < _gridWidth; x++)
        //     {
        //         Vector2Int pos = new Vector2Int(x, y);
        //         var slot = inventory.GetSlot(pos);
        //         _slotViews[x, y].SetItem(slot.item);
        //     }
        // }
    }
}
