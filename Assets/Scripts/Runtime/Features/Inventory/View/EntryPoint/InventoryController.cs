using System;
using UnityEngine;

namespace Runtime.Features.Inventory.View.EntryPoint
{
	public abstract class InventoryController : MonoBehaviour
	{
		public Action<bool> OnInventoryOpenStateChanged;
		public Action OnInventoryChanged;

		public abstract InventoryWithCells GetInventory();
	}
}