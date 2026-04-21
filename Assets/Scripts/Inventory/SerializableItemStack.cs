using System;

// Serializable representation of an ItemStack for saving/loading.
[Serializable]
public struct SerializableItemStack
{
    public string ItemId;
    public int Count;
    public int MaxStackSize;
}
