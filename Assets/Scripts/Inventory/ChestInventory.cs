using System.Collections.Generic;
// Chest wrapper around InventoryWithSlots
public class ChestInventory : IInventory
{
    public InventoryWithSlots Model { get; private set; }
    public string ChestId { get; private set; }
    public int Capacity => Model?.Capacity ?? 0;

    public ChestInventory(string chestId, int capacity)
    {
        ChestId = chestId;
        Model = new InventoryWithSlots(capacity);
    }
    public ItemStack GetSlot(int index) => Model.GetSlot(index);
    public void SetSlot(int index, ItemStack stack) => Model.SetSlot(index, stack);
    public bool AddItem(ItemStack stack) => Model.AddItem(stack);
    public ItemStack RemoveAt(int index) => Model.RemoveAt(index);
    public void Clear() => Model.Clear();
    public bool IsFull() => Model.IsFull();
    public bool CanMerge(ItemStack a, ItemStack b) => Model.CanMerge(a, b);
    public bool MoveItem(int fromSlot, IInventory target, int toSlot) => Model.MoveItem(fromSlot, target, toSlot);

    public List<SerializableItemStack> SerializeContents() => Model.SerializeContents();
    public void DeserializeContents(List<SerializableItemStack> data) => Model.DeserializeContents(data);
}
