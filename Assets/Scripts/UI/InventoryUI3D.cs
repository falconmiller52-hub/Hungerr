using System;
using System.Collections.Generic;
using UnityEngine;

// Universal 3D Inventory UI: can bind a chest inventory (wrapper) and a player inventory (IInventory)
public class InventoryUI3D : MonoBehaviour
{
    public Transform ChestRoot;
    public Transform PlayerRoot;
    public InventorySlot3DView SlotPrefab;

    private IInventory chestInv;
    private IInventory playerInv;

    private List<InventorySlot3DView> chestSlots = new List<InventorySlot3DView>();
    private List<InventorySlot3DView> playerSlots = new List<InventorySlot3DView>();

    private IInventory selectedInventory;
    private int selectedIndex = -1;

    public void BindInventories(IInventory chest, IInventory player)
    {
        chestInv = chest;
        playerInv = player;
        BuildUI();
    }

    private void BuildUI()
    {
        // Clear existing
        if (ChestRoot != null)
        {
            foreach (Transform t in ChestRoot) Destroy(t.gameObject);
        }
        if (PlayerRoot != null)
        {
            foreach (Transform t in PlayerRoot) Destroy(t.gameObject);
        }
        chestSlots.Clear();
        playerSlots.Clear();

        int chestCap = chestInv?.Capacity ?? 0;
        for (int i = 0; i < chestCap; i++)
        {
            var v = Instantiate(SlotPrefab, ChestRoot);
            v.BindSlot(chestInv, i);
            v.OnSlotClicked += OnSlotClicked;
            chestSlots.Add(v);
        }

        int playerCap = playerInv?.Capacity ?? 0;
        for (int i = 0; i < playerCap; i++)
        {
            var v = Instantiate(SlotPrefab, PlayerRoot);
            v.BindSlot(playerInv, i);
            v.OnSlotClicked += OnSlotClicked;
            playerSlots.Add(v);
        }
    }

    private void OnSlotClicked(IInventory inv, int index)
    {
        if (selectedInventory == null)
        {
            selectedInventory = inv;
            selectedIndex = index;
            HighlightSlot(inv, index, true);
        }
        else
        {
            if (inv == selectedInventory && index == selectedIndex)
            {
                ClearSelection();
                return;
            }
            bool moved = selectedInventory.MoveItem(selectedIndex, inv, index);
            ClearSelection();
            RefreshAll();
        }
    }

    private void HighlightSlot(IInventory inv, int index, bool on)
    {
        if (inv == chestInv)
            chestSlots[index]?.SetSelected(on);
        else if (inv == playerInv)
            playerSlots[index]?.SetSelected(on);
    }

    private void ClearSelection()
    {
        selectedInventory = null;
        selectedIndex = -1;
        foreach (var s in chestSlots) s.SetSelected(false);
        foreach (var s in playerSlots) s.SetSelected(false);
    }

    private void RefreshAll()
    {
        foreach (var s in chestSlots) s.Refresh();
        foreach (var s in playerSlots) s.Refresh();
    }
}
