using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.StateMachine;
using Zenject;

namespace Runtime.Infra.GameplayScene.GameplayStateMachine.States
{
	/// <summary>
	/// Exits the gameplay state, performs cleanup and transitions back to menu or other state.
	/// </summary>
	public class ExitGameplayState : IState
	{
		private readonly EventBus _eventBus;

		[Inject]
		public ExitGameplayState(EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		public void Enter()
		{
			// save game
			// clear subscriptions
			// release the addressables assets

			_eventBus.Trigger(EGameEvent.EndGameplay);
		}

		public void Exit()
		{
		}
	}
}