using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Features.Location;
using UnityEngine;

namespace Runtime.Features.DayNight.StateMachine
{
	public class DayPhaseState : GamePhaseState
	{
		public DayPhaseState(PhaseStateMachine owner, EventBus eventBus, ILoadingCurtain curtain, IInputHandler inputHandler, LocationChanger locationChanger) : base(owner, eventBus, curtain, inputHandler, locationChanger)
		{
		}

		public override void Enter()
		{
			Debug.Log("--- Наступил ДЕНЬ ---");
			
			Curtain.Show(0.01f, onEnd: OnCurtainShowEnded);
			
			EventBus.Subscribe(EGameplayChangeStateTriggerEvent.StartNightPhaseTrigger, StartNightPhase);
		}

		private void StartNightPhase()
		{
			Owner.EnterIn<NightPhaseState>();
		}

		private void OnCurtainShowEnded()
		{
			LocationChanger.ChangeLocation(Owner.DayStartLocationtransform, needCurtain: false);
			Owner.DayCycleVisualChanger.SetDay();
			
			EventBus.Trigger(EGameplayChangedStateEvent.OnEndNightPhase);
			
			Curtain.Hide(0.01f);
			InputHandler.Enable();
		}
		
		public override void Exit()
		{
			EventBus.Unsubscribe(EGameplayChangeStateTriggerEvent.StartNightPhaseTrigger, StartNightPhase);
		}
	}
}
