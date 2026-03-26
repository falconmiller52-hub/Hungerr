using System.Collections;
using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.LoadingCurtain;
using UnityEngine;

namespace Runtime.Features.DayNight.StateMachine
{
	public class DayPhaseState : GamePhaseState
	{
		public DayPhaseState(PhaseStateMachine owner, EventBus eventBus, ILoadingCurtain curtain, IInputHandler inputHandler) : base(owner, eventBus, curtain, inputHandler)
		{
		}

		public override void Enter()
		{
			Debug.Log("--- Наступил ДЕНЬ ---");
			
			Owner.DayCycleVisualChanger.SetDay();
			Curtain.Hide();
			InputHandler.Enable();
			
			EventBus.Subscribe(GameplayStateEvent.StartNightPhaseTrigger, StartNightPhase);
		}

		private void StartNightPhase()
		{
			Owner.StartCoroutine(ProcessNightPhase());
		}

		private IEnumerator ProcessNightPhase()
		{
			InputHandler.Disable();
			Curtain.Show();
			
			yield return new WaitForSeconds(0.7f);
			
			Owner.EnterIn<NightPhaseState>();
		}

		public override void Exit()
		{
			EventBus.Unsubscribe(GameplayStateEvent.StartNightPhaseTrigger, StartNightPhase);
		}
	}
}
