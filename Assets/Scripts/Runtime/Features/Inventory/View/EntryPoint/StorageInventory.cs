using System.Collections.Generic;
using Cinemachine;
using FMODUnity;
using Runtime.Common.Services.Audio.Sound;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Inventory.View.EntryPoint
{
	public class StorageInventory : InventoryController
	{
		[SerializeField] private EventReference _openInventorySound;
		[SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
		
		[Header("DEBUG")]
		[SerializeField] private InventoryItemData _inventoryItemData;
		[SerializeField] private InventoryItemData _inventoryItemDataTwo;
		[SerializeField] private Vector2Int _pos = Vector2Int.one;
		
		private InventoryWithCells _inventoryWithCells;
		private bool _isOpened;
		private int _width = 10;
		private int _height = 10;
		private ISoundService _soundService;

		[Inject]
		private void Construct(ISoundService soundService)
		{
			_soundService = soundService;
		}
		
		private void Awake()
		{
			if (!_isInitialized)
			{
				_inventoryWithCells = new InventoryWithCells(_width, _height);
				_isInitialized = true;
				
				Width = _width;
				Height = _height;
			}
		}
		
		public void InventoryOpenStateChanged(bool openState)
		{
			_soundService.PlaySound(_openInventorySound, transform.position);
			
			OnInventoryOpenStateChanged?.Invoke(openState);

			if (openState)
				EnableStorageCamera();
			else
				DisableStorageCamera();
		}

		private void EnableStorageCamera()
			=> _cinemachineVirtualCamera.Priority = 100;

		private void DisableStorageCamera()
			=> _cinemachineVirtualCamera.Priority = 0;
		
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

		public List<T> GetItems<T>() where T : InventoryItemData
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