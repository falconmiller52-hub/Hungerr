using System.Collections.Generic;
using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.SaveLoad;
using Runtime.Common.Services.StateMachine;
using Runtime.Features.DayNight.DaysCounter;
using Runtime.Features.Health;
using Runtime.Features.Inventory.View.EntryPoint;
using Runtime.Features.ItemSpawner;
using Runtime.Features.Player.Movement;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.GameplayScene.GameplayStateMachine.States
{
	/// <summary>
	/// Exits the gameplay state, performs cleanup and transitions back to menu or other state.
	/// </summary>
	public class ExitGameplayState : IState
	{
		private readonly EventBus _eventBus;
		private readonly ISaveLoadService _saveLoadService;

		[Inject]
		public ExitGameplayState(EventBus eventBus, ISaveLoadService saveLoadService)
		{
			_eventBus = eventBus;
			_saveLoadService = saveLoadService;
		}

		public void Enter()
		{
			// save game
			// clear subscriptions
			// release the addressables assets

			SaveGameStateData();

			_eventBus.Trigger(EGameEvent.EndGameplay);
		}

		private void SaveGameStateData()
		{
			GameStateData data = new GameStateData();
			
			
			GameObject playerInstance = Object.FindAnyObjectByType<PlayerMovement>().gameObject;

			if (playerInstance == null)
			{
				Debug.LogError("ExitGameplayState::SaveGameStateData() playerInstance is null");
				return;
			}
			
			//// сохраняем предметы игрока
			PlayerInventory playerInventory = playerInstance.GetComponentInChildren<PlayerInventory>();

			if (playerInventory == null)
			{
				Debug.LogError("ExitGameplayState::SaveGameStateData() playerInventory is null");
				return;
			}
			
			foreach (var slot in playerInventory.GetInventory().Slots)
			{
				if (slot.Value.IsEmpty || slot.Value.Id == -1)
					continue;

				var item = new InventoryItemSaveData();
				item.Position = new SerializableVector2Int(slot.Key);
				item.ItemDataID = slot.Value.Item.Data.Id;
				item.Amount = slot.Value.Item.Amount;
				data.PlayerInventoryItems.Add(item);
			}

			//// сохраняем предметы хранилища домового
			StorageInventory storageInventory = Object.FindAnyObjectByType<StorageInventory>();

			if (storageInventory == null)
			{
				Debug.LogError("ExitGameplayState::SaveGameStateData() storageInventory is null");
				return;
			}

			foreach (var slot in storageInventory.GetInventory().Slots)
			{
				if (slot.Value.IsEmpty || slot.Value.Id == -1)
					continue;
				
				var item = new InventoryItemSaveData();
				item.Position = new SerializableVector2Int(slot.Key);
				item.ItemDataID = slot.Value.Item.Data.Id;
				item.Amount = slot.Value.Item.Amount;
				data.StorageInventoryItems.Add(item);
			}
			
			//// сохраняем здоровье игрока
			PlayerHealth playerHealth = playerInstance.GetComponentInChildren<PlayerHealth>();

			if (playerHealth == null)
			{
				Debug.LogError("ExitGameplayState::SaveGameStateData() playerHealth is null");
				return;
			}
			
			data.Health = playerHealth.CurrentHealth;

			//// сохраняем предметы в мире из спавнеров
			ItemSpawner itemsSpawner = Object.FindAnyObjectByType<ItemSpawner>();

			if (itemsSpawner == null)
			{
				Debug.LogError("ExitGameplayState::SaveGameStateData() itemSpawer is null");
				return;
			}
			
			foreach (KeyValuePair<int, int> spawnPointData in itemsSpawner.GetSpawnPointsData())
			{
				var pointSaveData = new ItemSpawnPointSaveData();
				pointSaveData.ID = spawnPointData.Key;
				pointSaveData.ItemConfigId = spawnPointData.Value;

				data.SpawnPoints.Add(pointSaveData);
			}
			
			//// сохраняем текущий день
			CurrentDayController currentDayController = Object.FindAnyObjectByType<CurrentDayController>();

			if (currentDayController == null)
			{
				Debug.LogError("ExitGameplayState::SaveGameStateData() currentDayController is null");
				return;
			}
			
			data.CurrentDay = currentDayController.CurrentDay;
			
			_saveLoadService.SaveData(data);
		}

		public void Exit()
		{
		}
	}
}