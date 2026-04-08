// ItemInstance.cs

using UnityEngine;

namespace Runtime.Features.Inventory
{
	public class InventoryItem
	{
		public InventoryItemData _data;
		public int _amount;
    
		public InventoryItem(InventoryItemData itemData, int amount = 1)
		{
			_data = itemData;
			this._amount = Mathf.Min(amount, itemData.maxStackSize);
		}
    
		public bool CanStackWith(InventoryItem other)
		{
			return other != null && 
			       _data == other._data && 
			       _data.IsStackable && 
			       _amount + other._amount <= _data.maxStackSize;
		}
    
		public int SpaceAvailable()
		{
			return _data.maxStackSize - _amount;
		}
    
		public void Add(int amount)
		{
			this._amount = Mathf.Min(this._amount + amount, _data.maxStackSize);
		}
    
		public int Remove(int amount)
		{
			int removeAmount = Mathf.Min(amount, this._amount);
			this._amount -= removeAmount;
			return removeAmount;
		}
    
		public bool IsEmpty() => _amount <= 0;
	}
}