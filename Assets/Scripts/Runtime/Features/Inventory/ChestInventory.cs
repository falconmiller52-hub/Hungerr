using UnityEngine;

// Chest wrapper around InventoryWithCells
namespace Runtime.Features.Inventory
{
	public class ChestInventory : MonoBehaviour
	{
		[SerializeField] private int _width;
		[SerializeField] private int _height;

		private InventoryWithCells _inventoryWithCells;
		
		private void Start()
		{
			_inventoryWithCells = new InventoryWithCells(_width, _height);
		}

		public void Add()
		{
			
		}

		public void Remove()
		{
			
		}
	}
}
