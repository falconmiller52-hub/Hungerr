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
		private bool _isTimerActive;
		private TimeCounterUI _timeCounterUI = null;
		
		public NightPhaseState(PhaseStateMachine owner, EventBus eventBus, ILoadingCurtain curtain, IInputHandler inputHandler, LocationChanger locationChanger) : base(owner, eventBus, curtain, inputHandler, locationChanger)
		{
		}

		public override void Enter()
		{
			Debug.Log("--- Наступила НОЧЬ ---");
			
			_timeProgress = 0f;
			_timeCounterUI = Object.FindObjectOfType<TimeCounterUI>();
			
			if (_timeCounterUI != null)
			{
				_timeCounterUI.SetTimeState(true);
				_timeCounterUI.SetTime(_timeProgress);
			}
			
			Owner.StartCoroutine(ProcessStartNightPhase());
			
			EventBus.Subscribe(GameplayStateEvent.EndNightPhaseTrigger, EndNightPhase);

			_isTimerActive = true;
		}

		public override void Exit()
		{
			EventBus.Unsubscribe(GameplayStateEvent.EndNightPhaseTrigger, EndNightPhase);
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
		
		private IEnumerator ProcessStartNightPhase()
		{
			InputHandler.Disable();
			Curtain.Show(0);
			
			yield return new WaitForSeconds(0.4f); // заглушки
			LocationChanger.ChangeLocation(Owner.NightStartLocationtransform, needCurtain: false);
			yield return new WaitForSeconds(0.4f);
			
			Owner.DayCycleVisualChanger.SetNight();
			
			Curtain.Hide();
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