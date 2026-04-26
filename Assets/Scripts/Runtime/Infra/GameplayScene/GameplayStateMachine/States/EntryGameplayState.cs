using Runtime.Common.Factories.StateFactory;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.ResourceLoad;
using Runtime.Features.DayNight.StateMachine;
using Runtime.Features.Enemy;
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
		private readonly EnemiesBootstrap _enemiesBootstrap;

		[Inject]
		public EntryGameplayState(SceneStateMachine sceneStateMachine, PhaseStateMachine phaseStateMachine, 
			StateFactory stateFactory, InputHandler inputHandler, IResourceLoader resourceLoader, LocationChanger locationChanger,
			DiContainer diContainer, EnemiesBootstrap enemiesBootstrap)
		{
			_sceneStateMachine = sceneStateMachine;
			_phaseStateMachine = phaseStateMachine;
			_stateFactory = stateFactory;
			_inputHandler = inputHandler;
			_resourceLoader = resourceLoader;
			_locationChanger = locationChanger;
			_container = diContainer;
			_enemiesBootstrap = enemiesBootstrap;
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
			{
				Debug.LogError("EntryGameplayState::Enter() playerSpawnPoint is NULL");
			}

			GameObject playerInstance = _container.InstantiatePrefab(playerPrefab, 
				playerSpawnPoint ? playerSpawnPoint.transform.position : Vector3.one, 
				Quaternion.identity, 
				null);
			
			_locationChanger.Init(playerInstance.GetComponentInChildren<CharacterController>());
			
			_inputHandler.Init();
			_enemiesBootstrap.Init(playerInstance);
			
			// init PhaseStateMachine
			DayPhaseState dayPhaseState = _stateFactory.Create<DayPhaseState>();
			_phaseStateMachine.RegisterState(dayPhaseState);
			
			NightPhaseState nightPhaseState = _stateFactory.Create<NightPhaseState>();
			_phaseStateMachine.RegisterState(nightPhaseState);
			
			FirstDayPhaseState firstDayPhaseState = _stateFactory.Create<FirstDayPhaseState>();
			_phaseStateMachine.RegisterState(firstDayPhaseState);
			
			_phaseStateMachine.EnterIn<FirstDayPhaseState>();
			
			// Enter in main Gameplay State
			_sceneStateMachine.EnterIn<PlayGameplayState>();
		}
		
		public void Exit()
		{
		}
	}
}