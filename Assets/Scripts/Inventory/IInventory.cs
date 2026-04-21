using System.Collections.Generic;

// Core inventory interface shared by PlayerInventory and ChestInventory wrappers
public interface IInventory
{
    int Capacity { get; }
    ItemStack GetSlot(int index);
    void SetSlot(int index, ItemStack stack);
    bool AddItem(ItemStack stack);
    ItemStack RemoveAt(int index);
    void Clear();
    bool IsFull();
    bool CanMerge(ItemStack a, ItemStack b);
    bool MoveItem(int fromSlot, IInventory target, int toSlot);
    List<SerializableItemStack> SerializeContents();
    void DeserializeContents(List<SerializableItemStack> data);
}
