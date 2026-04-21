using System.Collections.Generic;
using UnityEngine;

// Core, slot-based inventory used by both PlayerInventory and ChestInventory wrappers
public class InventoryWithSlots
{
    protected ItemStack[] slots;

    public int Capacity => slots.Length;

    public InventoryWithSlots(int capacity)
    {
        if (capacity <= 0) capacity = 1;
        slots = new ItemStack[capacity];
    }

    public ItemStack GetSlot(int index)
    {
        if (index < 0 || index >= Capacity) return null;
        return slots[index];
    }

    public void SetSlot(int index, ItemStack stack)
    {
        if (index < 0 || index >= Capacity) return;
        slots[index] = stack;
    }

    // Add an item stack into the inventory, merging with existing stacks when possible.
    public bool AddItem(ItemStack stack)
    {
        if (stack == null || stack.Count <= 0) return true;

        // Merge with existing stacks of the same item
        for (int i = 0; i < Capacity; i++)
        {
            var s = slots[i];
            if (s != null && s.Item == stack.Item && s.Count < s.MaxStackSize)
            {
                int space = s.MaxStackSize - s.Count;
                int delta = Mathf.Min(space, stack.Count);
                s.Count += delta;
                stack.Count -= delta;
                if (stack.Count == 0) return true;
            }
        }

        // Put remaining into first empty slot
        for (int i = 0; i < Capacity; i++)
        {
            if (slots[i] == null)
            {
                ItemStack placed = new ItemStack
                {
                    Item = stack.Item,
                    Count = stack.Count,
                    MaxStackSize = stack.MaxStackSize
                };
                slots[i] = placed;
                stack.Count = 0;
                return true;
            }
        }

        // No space left
        return false;
    }

    // Remove stack from a specific slot
    public ItemStack RemoveAt(int index)
    {
        if (index < 0 || index >= Capacity) return null;
        var s = slots[index];
        slots[index] = null;
        return s;
    }

    // Clear all slots
    public void Clear()
    {
        for (int i = 0; i < Capacity; i++) slots[i] = null;
    }

    // Whether the inventory is full (no empty slots and no stack can be merged)
    public bool IsFull()
    {
        for (int i = 0; i < Capacity; i++)
        {
            var s = slots[i];
            if (s == null) return false;
            if (s.Count < s.MaxStackSize) return false;
        }
        return true;
    }

    // Check if two stacks can be merged
    public bool CanMerge(ItemStack a, ItemStack b)
    {
        if (a == null || b == null) return false;
        return a.Item == b.Item;
    }

    // Move a single stack from this inventory to a target inventory (destination slot)
    // Only moves if the entire stack can be placed in destination (no partial merges).
    public bool MoveItem(int fromSlot, IInventory target, int toSlot)
    {
        var stack = RemoveAt(fromSlot);
        if (stack == null) return false;

        var dest = target.GetSlot(toSlot);
        if (dest == null)
        {
            target.SetSlot(toSlot, stack);
            return true;
        }
        // Merge only if it fits entirely into the destination stack
        if (dest.Item == stack.Item && dest.Count + stack.Count <= dest.MaxStackSize)
        {
            dest.Count += stack.Count;
            return true;
        }

        // Cannot move fully, revert to original slot
        SetSlot(fromSlot, stack);
        return false;
    }

    // Serialization helper: convert contents to a serializable list
    public List<SerializableItemStack> SerializeContents()
    {
        var list = new List<SerializableItemStack>();
        for (int i = 0; i < Capacity; i++)
        {
            var s = slots[i];
            if (s != null && s.Count > 0)
            {
                list.Add(new SerializableItemStack
                {
                    ItemId = s.Item?.Id,
                    Count = s.Count,
                    MaxStackSize = s.MaxStackSize
                });
            }
        }
        return list;
    }

    // Deserialization helper (requires global ItemDatabase to resolve Item from Id)
    public void DeserializeContents(List<SerializableItemStack> data)
    {
        Clear();
        if (data == null) return;
        foreach (var di in data)
        {
            if (string.IsNullOrEmpty(di.ItemId)) continue;
            var item = ItemDatabase.GetItemById(di.ItemId);
            if (item == null) continue;
            var stack = new ItemStack
            {
                Item = item,
                Count = di.Count,
                MaxStackSize = di.MaxStackSize
            };
            AddItem(stack);
        }
    }
}
