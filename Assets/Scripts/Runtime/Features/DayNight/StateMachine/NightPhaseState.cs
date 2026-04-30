using Runtime.Common.Enums;
using Runtime.Common.Helpers;
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
		private bool _isTimerActive;
		private TimeCounterUI _timeCounterUI;
		
		public NightPhaseState(PhaseStateMachine owner, EventBus eventBus, ILoadingCurtain curtain, IInputHandler inputHandler, LocationChanger locationChanger) : base(owner, eventBus, curtain, inputHandler, locationChanger)
		{
		}

		public override void Enter()
		{
			Debug.Log("--- Наступила НОЧЬ ---");
			
			_timeProgress = 0f;
			_timeCounterUI = Object.FindAnyObjectByType<TimeCounterUI>();
			
			if (_timeCounterUI != null)
			{
				_timeCounterUI.SetTimeState(true);
				_timeCounterUI.SetTime(_timeProgress);
			}
			
			Owner.DayCycleVisualChanger.SetNight();
			
			_isTimerActive = true;
		}

		public override void Exit()
		{
			_isTimerActive = false;
			
			if (_timeCounterUI != null)
			{
				_timeCounterUI.SetTimeState(false);
				_timeCounterUI = null;
			}
		}
		
		public override void Update()
		{
			if (!_isTimerActive)
				return;
			
			_timeProgress += Time.deltaTime / Owner.NightDuration;

			if (_timeProgress >= 1f)
			{
				TriggerForceNightEnd();
				_isTimerActive = false;
				return;
			}
			
			Owner.DayCycleVisualChanger.UpdateNightCycle(_timeProgress);
			
			if (_timeCounterUI != null)
				_timeCounterUI.SetTime(_timeProgress);
		}

		private void TriggerForceNightEnd()
		{
			Debug.LogError("Игрок не успел домой до 6");
			
			StartDayTriggerEventData data = new StartDayTriggerEventData()
			{
				ForceNightEnd = true
			};
			
			EventBus.Trigger(EGameplayChangePhaseTriggerEvent.StartDayTrigger, data);
		}
	}
}