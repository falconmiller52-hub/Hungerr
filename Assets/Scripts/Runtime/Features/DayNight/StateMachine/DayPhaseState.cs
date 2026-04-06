using System.Collections;
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

			Owner.StartCoroutine(ProcessStartDayPhase());
			
			EventBus.Subscribe(EGameplayStateEvent.StartNightPhaseTrigger, StartNightPhase);
		}

		private void StartNightPhase()
		{
			Owner.EnterIn<NightPhaseState>();
			// Owner.StartCoroutine(ProcessNightPhase());
		}

		private IEnumerator ProcessStartDayPhase()
		{
			yield return null; // Заглушка чтобы 
			InputHandler.Disable();
			Curtain.Show(0);
			
			//ASK: Данный код спавнил игрока в своём месте, Я его закоментил чтобы он позицию игрока не переписывал позицию игрока
			/*yield return new WaitForSeconds(0.4f); // заглушки
			ChangeLocation(Owner.DayStartLocationtransform, needCurtain: false);
			yield return new WaitForSeconds(0.4f);*/
			
			Owner.DayCycleVisualChanger.SetDay();
			
			Curtain.Hide();
			InputHandler.Enable();
		}

		public override void Exit()
		{
			EventBus.Unsubscribe(EGameplayStateEvent.StartNightPhaseTrigger, StartNightPhase);
		}
	}
}
