using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Features.Inventory.View.EntryPoint
{
	public abstract class InventoryController : MonoBehaviour
	{
		public Action<bool> OnInventoryOpenStateChanged;
		public Action OnInventoryChanged;

		public int Width { get; protected set; }
		public int Height { get; protected set; }
		
		public abstract InventoryWithCells GetInventory();
		public abstract Dictionary<Vector2Int, InventorySlot> GetSlots();
	}
}