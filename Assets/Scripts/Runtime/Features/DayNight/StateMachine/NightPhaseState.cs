using System.Collections;
using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.LoadingCurtain;
using UnityEngine;

namespace Runtime.Features.DayNight.StateMachine
{
	public class NightPhaseState : GamePhaseState
	{
		private float _timeProgress;
		private bool _isTimerActive;
		
		public NightPhaseState(PhaseStateMachine owner, EventBus eventBus, ILoadingCurtain curtain, IInputHandler inputHandler) : base(owner, eventBus, curtain, inputHandler)
		{
		}

		public override void Enter()
		{
			Debug.Log("--- Наступила НОЧЬ ---");
			
			_timeProgress = 0f;
			
			Owner.DayCycleVisualChanger.SetNight();
			Curtain.Hide();
			InputHandler.Enable();
			
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
			Owner.StartCoroutine(ProcessEndNightPhase());
		}
		
		// заглушки
		private IEnumerator ProcessEndNightPhase()
		{
			InputHandler.Disable();
			Curtain.Show();
			
			yield return new WaitForSeconds(0.7f);
			
			Owner.EnterIn<DayPhaseState>();
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
		}

		private void TriggerDeath()
		{
			Debug.LogError("Игрок не успел спрятаться и погиб!");
			// Здесь можно вызвать переход в состояние Exit или вызвать экран смерти
		}
	}
}