// PlayerInventory.cs

using System;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Inventory
{
	/// <summary>
	/// это точка входа для инвентаря, такая штука может быть у сундуков или вот игрок
	/// тут мы создаем инвентарь и имеются методы с ккоторыми взаимодействуют другие скрипты игрока
	/// (например PlayerInteract мб чтоб взять предмет и положить)
	/// </summary>
	public class PlayerInventory : MonoBehaviour
	{
		[SerializeField] private InventoryItemData _inventoryItemData;
		[SerializeField] private InventoryItemData _inventoryItemDataTwo;
		[SerializeField] private Vector2Int _pos = Vector2Int.one;
		private InventoryWithCells _inventoryWithCells;
		private int width = 10;
		private int height = 10;
    
		public event Action OnInventoryChanged;
    
		[Inject]
		private void Construct()
		{
			_inventoryWithCells = new InventoryWithCells(width, height);
		}
    
		public bool AddItem(InventoryItem item, Vector2Int? position = null)
		{
			bool success = _inventoryWithCells.AddItem(item, position);
			if (success)
			{
				OnInventoryChanged?.Invoke();
			}
			return success;
		}
    
		// public bool RemoveItem(InventoryItemData data, int amount = 1)
		// {
		// 	if (_inventoryWithCells.TryTakeItem(data, out var taken, amount))
		// 	{
		// 		OnInventoryChanged?.Invoke();
		// 		return true;
		// 	}
		// 	return false;
		// }
    
		// public void DropItem(InventoryItem item, Vector3 worldPosition, Quaternion rotation)
		// {
		// 	RemoveItem(item._data, item._amount);
		// 	// тут типа спавн выброшенного предмета в 3д мир и тд
		// 	// _itemWorldManager.SpawnItemInWorld(item._data, item._amount, worldPosition, rotation);
		// 	OnInventoryChanged?.Invoke();
		// }
    
		public InventoryWithCells GetInventory() => _inventoryWithCells;

		[ContextMenu("Add Item")]
		public void AddItem()
		{
			var s = new InventoryItem(_inventoryItemData);
			AddItem(s, _pos);
		}
		
		[ContextMenu("Add Item without pos")]
		public void AddItemNoPos()
		{
			var s = new InventoryItem(_inventoryItemDataTwo);
			AddItem(s);
		}
	}
}