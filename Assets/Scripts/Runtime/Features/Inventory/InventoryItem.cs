using UnityEngine;

namespace Runtime.Features.Inventory
{
	public class InventoryItem
	{
		public InventoryItemData Data;
		public int Amount;
    
		public InventoryItem(InventoryItemData itemData, int amount = 1)
		{
			Data = itemData;
			Amount = Mathf.Min(amount, itemData.MaxStackSize);
		}
    
		public bool CanStackWith(InventoryItem other)
		{
			return other != null && 
			       Data == other.Data && 
			       Data.IsStackable && 
			       Amount + other.Amount <= Data.MaxStackSize;
		}
    
		public int SpaceAvailable()
		{
			return Data.MaxStackSize - Amount;
		}
    
		public void Add(int amount)
		{
			Amount = Mathf.Min(Amount + amount, Data.MaxStackSize);
		}
    
		public int Remove(int amount)
		{
			int removeAmount = Mathf.Min(amount, Amount);
			Amount -= removeAmount;
			
			return removeAmount;
		}
    
		public bool IsEmpty() => Amount <= 0;
	}
}