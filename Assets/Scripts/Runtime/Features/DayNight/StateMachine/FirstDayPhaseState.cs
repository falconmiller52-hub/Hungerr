using System;
using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Features.Location;
using Runtime.Features.Player.Other;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.Features.DayNight.StateMachine
{
	public class FirstDayPhaseState : GamePhaseState
	{
		public FirstDayPhaseState(PhaseStateMachine owner, EventBus eventBus, ILoadingCurtain curtain, IInputHandler inputHandler, LocationChanger locationChanger) : base(owner, eventBus, curtain, inputHandler, locationChanger)
		{
		}

		public override void Enter()
		{
			Debug.Log("--- Наступил ПЕРВЫЙ ДЕНЬ ---");
			
			Curtain.Show(0.01f, onEnd: OnCurtainShowEnded);
			
			EventBus.Subscribe(EGameplayStateEvent.StartNightPhaseTrigger, StartNightPhase);
		}

		private void StartNightPhase()
		{
			Owner.EnterIn<NightPhaseState>();
		}

		private void OnCurtainShowEnded()
		{
			Owner.DayCycleVisualChanger.SetDay();
			
			Curtain.Hide(0.01f);
			InputHandler.Enable();
		}
		
		public override void Exit()
		{
			EventBus.Unsubscribe(EGameplayStateEvent.StartNightPhaseTrigger, StartNightPhase);
		}
	}
}