using Runtime.Common.Enums;
using Runtime.Common.Helpers;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Common.Services.StateMachine;
using Runtime.Features.DayNight.DaysCounter;
using Runtime.Features.DayNight.StateMachine;
using Runtime.Features.Enemy;
using Runtime.Features.Enemy.Domovoi;
using Runtime.Features.Location;
using Runtime.Features.Player.Other;
using Runtime.Features.Supervision;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.GameplayScene.GameplayStateMachine.States
{
	/// <summary>
	/// Gameplay state where the actual play scene is active.
	/// </summary>
	public class PlayGameplayState : IState
	{
		private readonly SceneStateMachine _sceneStateMachine;
		private readonly PhaseStateMachine _phaseStateMachine;
		private readonly InputHandler _inputHandler;
		private readonly LocationChanger _locationChanger;
		private readonly EventBus _eventBus;
		private readonly ILoadingCurtain _curtain;
		private readonly EnemiesController _enemiesController;

		private DomovoiAI _domovoiAI;
		private SupervisionController _supervisionController;
		private PlayerFoodController _playerFoodController;

		private int _currentDay;

		[Inject]
		public PlayGameplayState(SceneStateMachine sceneStateMachine, PhaseStateMachine phaseStateMachine,
			InputHandler inputHandler, LocationChanger locationChanger, EventBus eventBus, ILoadingCurtain curtain, EnemiesController enemiesController)
		{
			_sceneStateMachine = sceneStateMachine;
			_phaseStateMachine = phaseStateMachine;
			_inputHandler = inputHandler;
			_locationChanger = locationChanger;
			_eventBus = eventBus;
			_enemiesController = enemiesController;
			_curtain = curtain;
		}

		public void Enter()
		{
			_domovoiAI = Object.FindAnyObjectByType<DomovoiAI>();
			_supervisionController = Object.FindAnyObjectByType<SupervisionController>();
			_playerFoodController = Object.FindAnyObjectByType<PlayerFoodController>();

			CurrentDayController currentDayController = Object.FindAnyObjectByType<CurrentDayController>();
			_currentDay = currentDayController.CurrentDay;
			
			_eventBus.Subscribe<EGameplayChangePhaseTriggerEvent, StartDayTriggerEventData>(EGameplayChangePhaseTriggerEvent.StartDayTrigger, StartDayPhaseTriggered);
			_eventBus.Subscribe(EGameplayChangePhaseTriggerEvent.StartNightTrigger, StartNightPhaseTriggered);
			
			StartDayPhaseTriggered(new StartDayTriggerEventData() { ForceNightEnd = false });
		}

		public void Execute() { }

		/// <summary>
		/// хендлит начало дня:
		/// - включает визуал дня
		/// - высчитывает есть ли наказания
		/// - перемещает игрока на точку (если нет наказаний)
		/// - триггерим что день начался и все могут колбеком делать штуки
		/// </summary>
		/// <param name="data"></param>
		private void StartDayPhaseTriggered(StartDayTriggerEventData data)
		{
			_playerFoodController.SetActiveFoodDrain(false);
			
			// ВЫКЛючаем инпут
			_inputHandler.Disable();
			// ВКЛючаем шторку
			_curtain.Show(onEnd: OnStartDayPhaseTriggered);

			void OnStartDayPhaseTriggered()
			{
				_phaseStateMachine.EnterIn<DayPhaseState>();

				(bool, EDomovoiSatietyLevel) domovoiData = _domovoiAI.StartDayPhaseHandler();

				if (data.ForceNightEnd)
				{
					_supervisionController.OnLateAtNight();
				}
				else if (domovoiData.Item1)
				{
					_supervisionController.OnDomovoiDontFed();
				}
				else
				{
					_supervisionController.ClearAllPunishObjects();
					_locationChanger.ChangeLocation(_phaseStateMachine.DayStartLocationtransform);
					_eventBus.Trigger(domovoiData.Item2);
				}

				_enemiesController.SetAllEnemiesToSpawnPoint();
				
				_eventBus.Trigger(EGameplayChangedPhaseEvent.DayStarted);

				// ВЫКЛючаем шторку
				_curtain.Hide();
				// ВКЛючаем инпут gameplay
				_inputHandler.SwitchToGameplayMap();
			}
		}

		/// <summary>
		/// хендлит начало ночной фазы:
		/// - перемещает игрока на точку спавна ночью
		/// - включает визуал ночи
		/// - триггерим что ночь началась и все могут колбеком делать штуки
		/// </summary>
		private void StartNightPhaseTriggered()
		{
			// ВЫКЛючаем инпут
			_inputHandler.Disable();
			// ВКЛючаем шторку
			_curtain.Show(onEnd: OnStartNightPhaseTriggered);

			void OnStartNightPhaseTriggered()
			{
				_currentDay++;
				_domovoiAI.StartNightPhaseHandler(_currentDay);

				var startPhaseData = new StartNightEventData();
				startPhaseData.CurrentDay = _currentDay;

				_locationChanger.ChangeLocation(_phaseStateMachine.NightStartLocationtransform);

				_phaseStateMachine.EnterIn<NightPhaseState>();

				_eventBus.Trigger(EGameplayChangedPhaseEvent.NightStarted, startPhaseData);

				// ВЫКЛючаем шторку
				_curtain.Hide();
				// ВКЛючаем инпут gameplay
				_inputHandler.SwitchToGameplayMap();
				
				_playerFoodController.SetActiveFoodDrain(true);
			}
		}

		public void Exit()
		{
			_eventBus.Unsubscribe<EGameplayChangePhaseTriggerEvent, StartDayTriggerEventData>(EGameplayChangePhaseTriggerEvent.StartDayTrigger, StartDayPhaseTriggered);
			_eventBus.Unsubscribe(EGameplayChangePhaseTriggerEvent.StartNightTrigger, StartNightPhaseTriggered);
		}
	}
}