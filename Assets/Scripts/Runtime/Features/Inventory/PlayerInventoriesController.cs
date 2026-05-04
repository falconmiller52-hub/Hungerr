using Runtime.Common.Services.Audio;
using Runtime.Common.Services.Audio.Sound;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.Pause;
using Runtime.Features.Inventory.View;
using Runtime.Features.Inventory.View.EntryPoint;
using Runtime.Features.Inventory.View.UIHelpers;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Inventory
{
	/// <summary>
	/// это скрипт для менеджмента окон инвентаря и хранилищ
	/// тут имеются методы для открытия\закрытия 
	/// </summary>
	public class PlayerInventoriesController : MonoBehaviour
	{
		[SerializeField] private PlayerInventory _playerInventory;
		[SerializeField] private ItemDragger _itemDragger;

		private IInputHandler _inputHandler;
		private IPauseController _pauseController;
		private ISoundService _soundService;
		private bool _isOpened;
		private StorageInventory _currentOpenedStorage;

		[Inject]
		private void Construct(IInputHandler inputHandler, IPauseController pauseController, ISoundService soundService)
		{
			_inputHandler = inputHandler;
			_pauseController = pauseController;
			_soundService = soundService;
		}

		private void Start()
		{
			_inputHandler.InventoryTriggerPressed += OnPlayerInventoryTriggerPressed;
		}

		private void OnDisable()
		{
			_inputHandler.InventoryTriggerPressed -= OnPlayerInventoryTriggerPressed;
		}

		private void OnPlayerInventoryTriggerPressed()
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

				if (_currentOpenedStorage != null)
				{
					_currentOpenedStorage.InventoryOpenStateChanged(_isOpened);
					_currentOpenedStorage = null;
					_itemDragger.CloseChest();
				}
			}

			_playerInventory.InventoryOpenStateChanged(_isOpened);
		}

		public void OpenStorage(StorageInventory storage, Inventory3DView inventoryView)
		{
			if (storage == null || _playerInventory == null)
				return;

			_currentOpenedStorage = storage;
			_isOpened = true;

			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			_pauseController.PerformStop();

			_playerInventory.InventoryOpenStateChanged(_isOpened);
			_currentOpenedStorage.InventoryOpenStateChanged(_isOpened);
			_itemDragger.OpenChest(inventoryView);
		}
	}
}