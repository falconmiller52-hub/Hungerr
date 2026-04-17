using UnityEngine;

namespace Runtime.Features.Inventory
{
	public class InventorySlot
	{
		public InventoryItem Item;
		public Vector2Int Position; // позиция в инвентаре (x, y)
		public int Id;
		
		public InventorySlot(Vector2Int pos)
		{
			Position = pos;
			Id = -1;
		}
    
		public bool IsEmpty => Item == null;
    
		public bool CanStackWith(InventoryItem other)
		{
			return !IsEmpty && Item.CanStackWith(other);
		}
    
		public bool TryAddItem(InventoryItem newItem, int id)
		{
			if (IsEmpty)
			{
				Item = newItem;
				Id = id;
				return true;
			}
        
			if (CanStackWith(newItem))
			{
				int space = Item.SpaceAvailable();
				
				if (newItem.Amount <= space)
				{
					Item.Add(newItem.Amount);
					Id = id;
					
					return true;
				}

				Item.Add(space);
				newItem.Remove(space);
				Id = id;
				
				return true;
			}
        
			return false;
		}
    
		public int RemoveItem(int amount = 1)
		{
			if (IsEmpty)
				return 0;
            
			int removed = Item.Remove(amount);
			if (Item.IsEmpty())
				Item = null;
            
			return removed;
		}
	}
}