using Runtime.Common.Enums;
using Runtime.Common.Helpers;
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
			
			Owner.DayCycleVisualChanger.SetDay();
		}
		
		public override void Exit()
		{
		}
	}
}
