using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.SaveLoad;
using Runtime.Common.Services.StateMachine;
using Runtime.Features.Health;
using Runtime.Features.Inventory;
using Runtime.Features.Inventory.View.EntryPoint;
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
			GameObject playerInstance = Object.FindAnyObjectByType<PlayerMovement>().gameObject;

			if (playerInstance == null)
			{
				Debug.LogError("ExitGameplayState::SaveGameStateData() playerInstance is null");
				return;
			}

			GameStateData data = new GameStateData();
			
			PlayerInventory playerInventory = playerInstance.GetComponentInChildren<PlayerInventory>();
			
			foreach (var slot in playerInventory.GetInventory().Slots)
			{
				if (slot.Value.IsEmpty || slot.Value.Id == -1)
					continue;
				
				var item = new SlotSaveData();
				item.Position = new SerializableVector2Int(slot.Key);
				item.ItemDataID = slot.Value.Item.Data.Id;
				item.Amount = slot.Value.Item.Amount;
				data.Slots.Add(item);
			}
			
			PlayerHealth playerHealth = playerInstance.GetComponentInChildren<PlayerHealth>();
			data.Health = playerHealth.CurrentHealth;
			
			_saveLoadService.SaveData(data);
		}

		public void Exit()
		{
		}
	}
}