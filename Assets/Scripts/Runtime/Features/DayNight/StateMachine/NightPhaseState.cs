using System.Collections;
using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Features.Location;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Features.DayNight.StateMachine
{
	public class NightPhaseState : GamePhaseState
	{
		private float _timeProgress;
		private int _currentDay;
		private bool _isTimerActive;
		private TimeCounterUI _timeCounterUI = null;
		
		public NightPhaseState(PhaseStateMachine owner, EventBus eventBus, ILoadingCurtain curtain, IInputHandler inputHandler, LocationChanger locationChanger) : base(owner, eventBus, curtain, inputHandler, locationChanger)
		{
			// пока даты игрока нет, будем считать что начинается с 1 дня (обновление в Enter)
			_currentDay = 0;
		}

		public override void Enter()
		{
			Debug.Log("--- Наступила НОЧЬ ---");
			
			_timeProgress = 0f;
			_currentDay += 1;
			_timeCounterUI = Object.FindAnyObjectByType<TimeCounterUI>();
			
			if (_timeCounterUI != null)
			{
				_timeCounterUI.SetTimeState(true);
				_timeCounterUI.SetTime(_timeProgress);
			}
			
			Curtain.Show(0.01f, onEnd: OnCurtainShowEnded);
			
			EventBus.Subscribe(EGameplayChangeStateTriggerEvent.EndNightPhaseTrigger, EndNightPhase);
		}

		public override void Exit()
		{
			EventBus.Unsubscribe(EGameplayChangeStateTriggerEvent.EndNightPhaseTrigger, EndNightPhase);
		}
		
		private void EndNightPhase()
		{
			_isTimerActive = false;
			
			if (_timeCounterUI != null)
			{
				_timeCounterUI.SetTimeState(false);
				_timeCounterUI = null;
			}

			Owner.EnterIn<DayPhaseState>();
		}
		
		private void OnCurtainShowEnded()
		{
			LocationChanger.ChangeLocation(Owner.NightStartLocationtransform, needCurtain: false);
			Owner.DayCycleVisualChanger.SetNight();
			
			EventBus.Trigger(EGameplayChangedStateEvent.OnStartNightPhase, _currentDay);
			
			_isTimerActive = true;
			
			Curtain.Hide(0.01f);
			InputHandler.Enable();
		}

		public override void Update()
		{
			if (!_isTimerActive)
				return;
			
			_timeProgress += Time.deltaTime / Owner.NightDuration;

			if (_timeProgress >= 1f)
			{
				TriggerDeath();
				EndNightPhase();
				return;
			}
			
			Owner.DayCycleVisualChanger.UpdateNightCycle(_timeProgress);
			
			if (_timeCounterUI != null)
				_timeCounterUI.SetTime(_timeProgress);
		}

		private void TriggerDeath()
		{
			Debug.LogError("Игрок не успел спрятаться и погиб!");
			// Здесь можно вызвать переход в состояние Exit или вызвать экран смерти
		}
	}
}