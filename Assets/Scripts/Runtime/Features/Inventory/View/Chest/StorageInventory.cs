using FMODUnity;
using Runtime.Common.Services.Audio;
using Runtime.Features.Inventory.View.EntryPoint;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Inventory.View.Chest
{
	public class StorageInventory : InventoryController
	{
		[SerializeField] private EventReference _openInventorySound;
		
		[Header("DEBUG")]
		[SerializeField] private InventoryItemData _inventoryItemData;
		[SerializeField] private InventoryItemData _inventoryItemDataTwo;
		[SerializeField] private Vector2Int _pos = Vector2Int.one;
		
		private InventoryWithCells _inventoryWithCells;
		private int _width = 5;
		private int _height = 5;
		private bool _isOpened;
		private IAudioService _audioService;

		[Inject]
		private void Construct(IAudioService audioService)
		{
			_audioService = audioService;
		}
		
		private void Start()
		{
			_inventoryWithCells = new InventoryWithCells(_width, _height);
		}


		public void InventoryOpenStateChanged(bool openState)
		{
			_audioService.PlaySound(_openInventorySound, transform.position);
			
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
		
		public override InventoryWithCells GetInventory() => _inventoryWithCells;
    
		
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