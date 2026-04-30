using System.Collections.Generic;
using System.Linq;
using Runtime.Common.Factories.StateFactory;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.ItemsIdentifier;
using Runtime.Common.Services.ResourceLoad;
using Runtime.Common.Services.SaveLoad;
using Runtime.Features.DayNight.DaysCounter;
using Runtime.Features.DayNight.StateMachine;
using Runtime.Features.Enemy;
using Runtime.Features.GameOver;
using Runtime.Features.Health;
using Runtime.Features.Inventory;
using Runtime.Features.Inventory.View.EntryPoint;
using Runtime.Features.ItemSpawner;
using Runtime.Features.Location;
using Runtime.Features.Player.Other;
using UnityEngine;
using Zenject;
using IState = Runtime.Common.Services.StateMachine.IState;
using Object = UnityEngine.Object;

namespace Runtime.Infra.GameplayScene.GameplayStateMachine.States
{
	/// <summary>
	/// Entry point for the gameplay state; initializes scene components and transitions to Play state.
	/// </summary>
	public class EntryGameplayState : IState
	{
		private readonly SceneStateMachine _sceneStateMachine;
		private readonly PhaseStateMachine _phaseStateMachine;
		private readonly StateFactory _stateFactory;
		private readonly InputHandler _inputHandler;
		private readonly IResourceLoader _resourceLoader;
		private readonly LocationChanger _locationChanger;
		private readonly DiContainer _container;
		private readonly EnemiesController _enemiesController;
		private readonly ISaveLoadService _saveLoadService;
		private readonly ItemsIdentifierSO _identifierSO;

		private GameOverTriggerHandler _gameOverTriggerHandler;

		// saved data refs
		Dictionary<string, int> _spawnPointsSaveData = new Dictionary<string, int>();
		int _currentDay = 0;


		[Inject]
		public EntryGameplayState(SceneStateMachine sceneStateMachine, PhaseStateMachine phaseStateMachine,
						StateFactory stateFactory, InputHandler inputHandler, IResourceLoader resourceLoader,
						LocationChanger locationChanger,
						DiContainer diContainer, EnemiesController enemiesController, ISaveLoadService saveLoadService,
						ItemsIdentifierSO itemsIdentifierSO)
		{
			_sceneStateMachine = sceneStateMachine;
			_phaseStateMachine = phaseStateMachine;
			_stateFactory = stateFactory;
			_inputHandler = inputHandler;
			_resourceLoader = resourceLoader;
			_locationChanger = locationChanger;
			_container = diContainer;
			_enemiesController = enemiesController;
			_saveLoadService = saveLoadService;
			_identifierSO = itemsIdentifierSO;
		}

		public void Enter()
		{
			// Выключили курсор при старте игры
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			// заглушка пока нет адресаблов
			GameObject playerPrefab = _resourceLoader.Load<GameObject>("Player");
			PlayerSpawnPoint playerSpawnPoint = Object.FindFirstObjectByType<PlayerSpawnPoint>();

			if (playerSpawnPoint == null)
				Debug.LogError("EntryGameplayState::Enter() playerSpawnPoint is NULL");

			GameObject playerInstance = _container.InstantiatePrefab(playerPrefab,
							playerSpawnPoint ? playerSpawnPoint.transform.position : Vector3.one,
							Quaternion.identity,
							null);

			////

			ItemSpawner itemSpawner = Object.FindAnyObjectByType<ItemSpawner>();
			StorageInventory storageInventory = Object.FindAnyObjectByType<StorageInventory>();
			storageInventory.InitModel();

			GameStateData loadedData = _saveLoadService.LoadData();

			if (loadedData != null && playerInstance != null)
				ApplyLoadedData(loadedData, playerInstance, storageInventory);

			if (_spawnPointsSaveData.Count > 0)
				itemSpawner.SpawnItems(_spawnPointsSaveData);
			else
				itemSpawner.SpawnItems();

			////

			_locationChanger.Init(playerInstance.GetComponentInChildren<CharacterController>());

			_inputHandler.Init();
			_enemiesController.Init(playerInstance);

			_container.Instantiate<GameOverTriggerHandler>();

			// init PhaseStateMachine
			DayPhaseState dayPhaseState = _stateFactory.Create<DayPhaseState>();
			_phaseStateMachine.RegisterState(dayPhaseState);

			NightPhaseState nightPhaseState = _stateFactory.Create<NightPhaseState>();
			_phaseStateMachine.RegisterState(nightPhaseState);

			FirstDayPhaseState firstDayPhaseState = _stateFactory.Create<FirstDayPhaseState>();
			_phaseStateMachine.RegisterState(firstDayPhaseState);

			if (_currentDay == 0)
				_phaseStateMachine.EnterIn<FirstDayPhaseState>();
			else
				_phaseStateMachine.EnterIn<DayPhaseState>();

			// Enter in main Gameplay State
			_sceneStateMachine.EnterIn<PlayGameplayState>();
		}

		public void Execute()
		{
		}

		public void Exit()
		{
		}

		private void ApplyLoadedData(GameStateData loadedData, GameObject playerInstance,
						StorageInventory storageInventory)
		{
			if (loadedData == null)
				return;

			// проверяем и загружаем предметы игрока
			if (loadedData.PlayerInventoryItems != null && loadedData.PlayerInventoryItems.Count > 0 &&
			    playerInstance != null)
			{
				PlayerInventory playerInventory = playerInstance.GetComponent<PlayerInventory>();
				// убеждаемся что инвентарь точно есть
				playerInventory.InitInventoryModel();
				foreach (var saveItem in loadedData.PlayerInventoryItems)
				{
					var item = new InventoryItem(_identifierSO.GetItemDataById(saveItem.ItemDataID), saveItem.Amount);
					playerInventory.AddItem(item, saveItem.Position.ToVector());
				}
			}

			// проверяем и загружаем предметы в хранилище для домового
			if (loadedData.StorageInventoryItems != null && loadedData.StorageInventoryItems.Count > 0)
			{
				foreach (var saveItem in loadedData.StorageInventoryItems)
				{
					var item = new InventoryItem(_identifierSO.GetItemDataById(saveItem.ItemDataID), saveItem.Amount);
					storageInventory.AddItem(item, saveItem.Position.ToVector());
				}
			}

			// проверяем и загружаем состояние здоровья
			if (loadedData.PlayerHealth > 0)
			{
				var playerHealth = playerInstance.GetComponent<PlayerHealth>();
				playerHealth.SetHealth(loadedData.PlayerHealth);
			}

			// Проверяем и загружаем голод игрока
			if (loadedData.PlayerHunger > 0)
			{
				PlayerFoodController playerFoodController = playerInstance.GetComponent<PlayerFoodController>();
				playerFoodController.SetFood(loadedData.PlayerHunger);
			}

			// проверяем и загружаем предметы на спавн поинтах
			if (loadedData.SpawnPoints != null && _spawnPointsSaveData != null)
			{
				_spawnPointsSaveData = loadedData.SpawnPoints.ToDictionary(
								rvp => rvp.ID,
								rvp => rvp.ItemConfigId
				);
			}

			CurrentDayController currentDayController = Object.FindAnyObjectByType<CurrentDayController>();

			if (currentDayController != null)
				currentDayController.Init(loadedData.CurrentDay);
			else
				Debug.LogError("EntryGameplayState::Enter() currentDayController is null");
		}
	}
}