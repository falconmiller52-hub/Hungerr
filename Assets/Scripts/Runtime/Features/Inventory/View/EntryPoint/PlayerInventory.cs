using System;
using FMODUnity;
using Runtime.Common.Services.Audio;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.Pause;
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
		[SerializeField] private EventReference _openInventorySound;
		
		[Header("DEBUG")]
		[SerializeField] private InventoryItemData _inventoryItemData;
		[SerializeField] private InventoryItemData _inventoryItemDataTwo;
		[SerializeField] private Vector2Int _pos = Vector2Int.one;

		public event Action<bool> OnInventoryOpenStateChanged;
		public event Action OnInventoryChanged;
		
		private InventoryWithCells _inventoryWithCells;
		private IInputHandler _inputHandler;
		private int _width = 10;
		private int _height = 10;
		private bool _isOpened;
		private IPauseController _pauseController;
		private IAudioService _audioService;

		[Inject]
		private void Construct(IInputHandler inputHandler, IPauseController pauseController, IAudioService audioService)
		{
			_inputHandler = inputHandler;
			_pauseController = pauseController;
			_audioService = audioService;
		}
		
		private void Start()
		{
			_inventoryWithCells = new InventoryWithCells(_width, _height);
			_inputHandler.InventoryTriggerPressed += OnInventoryTriggerPressed;
		}

		private void OnDisable()
		{
			_inputHandler.InventoryTriggerPressed -= OnInventoryTriggerPressed;
		}

		private void OnInventoryTriggerPressed()
		{
			_isOpened = !_isOpened;
			
			if (_isOpened)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				_pauseController.PerformStop();
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				_pauseController.PerformResume();
			}
			
			_audioService.PlaySound(_openInventorySound, transform.position);
			
			OnInventoryOpenStateChanged?.Invoke(_isOpened);
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
    
		
		
		// public void DropItem(InventoryItem item, Vector3 worldPosition, Quaternion rotation)
		// {
		// 	RemoveItem(item._data, item._amount);
		// 	// тут типа спавн выброшенного предмета в 3д мир и тд
		// 	// _itemWorldManager.SpawnItemInWorld(item._data, item._amount, worldPosition, rotation);
		// 	OnInventoryChanged?.Invoke();
		// }
		
		public InventoryWithCells GetInventory() => _inventoryWithCells;
    
		
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