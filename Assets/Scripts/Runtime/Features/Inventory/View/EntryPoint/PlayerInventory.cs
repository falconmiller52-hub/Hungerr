using System;
using System.Collections.Generic;
using FMODUnity;
using Runtime.Common.Services.Audio.Sound;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Inventory.View.EntryPoint
{
	/// <summary>
	/// это controller инвентаря, такая штука может быть у сундуков или у игрока
	/// тут мы создаем модель инвентаря и имеются методы для работы с инвентарем
	/// </summary>
	public class PlayerInventory : InventoryController
	{
		[SerializeField] private EventReference _openInventorySound;
		
		[Header("DEBUG")]
		[SerializeField] private InventoryItemData _inventoryItemData;
		[SerializeField] private InventoryItemData _inventoryItemDataTwo;
		[SerializeField] private Vector2Int _pos = Vector2Int.one;
		
		private InventoryWithCells _inventoryWithCells;
		private int _width = 10;
		private int _height = 10;
		private bool _isOpened;
		private ISoundService _soundService;

		[Inject]
		private void Construct(ISoundService soundService)
		{
			_soundService = soundService;
		}

		private void Start()
		{
			InitInventoryModel();
		}

		public void InitInventoryModel()
		{
			if (_inventoryWithCells == null)
				_inventoryWithCells = new InventoryWithCells(_width, _height);
			
			Width = _width;
			Height = _height;
		}
		
		public void InventoryOpenStateChanged(bool openState)
		{
			_soundService.PlaySound(_openInventorySound, transform.position);
			
			OnInventoryOpenStateChanged?.Invoke(openState);
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
    
		public bool RemoveItem(Vector2Int pos, int amount = 1)
		{
			if (_inventoryWithCells.RemoveItemByPosition(pos, amount))
			{
				OnInventoryChanged?.Invoke();
				return true;
			}
			
			return false;
		}
		
		public void RemoveAllItemsByType<T>() where T : InventoryItemData
		{
			_inventoryWithCells.RemoveAllItemsByType<T>();
			
			OnInventoryChanged?.Invoke();
		}

		public List<InventoryItem> GetItems<T>() where T : InventoryItemData
		{
			return _inventoryWithCells.GetItems<T>();
		}
		
		public override InventoryWithCells GetInventory() => _inventoryWithCells;
		
		public override Dictionary<Vector2Int, InventorySlot> GetSlots()
		{
			return _inventoryWithCells.Slots;
		}


		// DEBUG

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
		
		[ContextMenu("Remove Item")]
		public void RemoveItem()
		{
			var s = new InventoryItem(_inventoryItemData);
			RemoveItem(_pos);
		}
	}
}