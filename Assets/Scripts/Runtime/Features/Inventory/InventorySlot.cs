// InventorySlot.cs

using System;
using UnityEngine;

namespace Runtime.Features.Inventory
{
	[Serializable]
	public class InventorySlot
	{
		public InventoryItem item;
		public Vector2Int position; // позиция в инвентаре (x, y)
		public bool IsEmpty => item == null;
    
		public InventorySlot(Vector2Int pos)
		{
			position = pos;
		}
    
		public bool CanPlaceItem(InventoryItem newItem)
		{
			if (IsEmpty)
				return true;
            
			return CanStackWith(newItem);
		}
    
		public bool CanStackWith(InventoryItem other)
		{
			return !IsEmpty && item.CanStackWith(other);
		}
    
		public bool TryAddItem(InventoryItem newItem)
		{
			if (IsEmpty)
			{
				item = newItem;
				return true;
			}
        
			if (CanStackWith(newItem))
			{
				int space = item.SpaceAvailable();
				if (newItem._amount <= space)
				{
					item.Add(newItem._amount);
					return true;
				}
				else
				{
					item.Add(space);
					newItem.Remove(space);
					return true;
				}
			}
        
			return false;
		}
    
		public int RemoveItem(int amount = 1)
		{
			if (IsEmpty)
				return 0;
            
			int removed = item.Remove(amount);
			if (item.IsEmpty())
				item = null;
            
			return removed;
		}
	}
}