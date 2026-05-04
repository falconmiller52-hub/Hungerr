using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.StateMachine;

namespace Runtime.Infra.App.GlobalStateMachine.States
{
	/// <summary>
	///     Core gameplay loop state tag
	/// </summary>
	public class GameplayLoopState : IState
	{
		private EventBus _eventBus;
		private GlobalStateMachine _globalStateMachine;

		public GameplayLoopState(EventBus eventBus, GlobalStateMachine globalStateMachine)
		{
			_eventBus = eventBus;
			_globalStateMachine = globalStateMachine;
		}

		public void Enter()
		{
			_eventBus.Subscribe(EGameEvent.EndGameplay, EnterInGameMenuState);
		}

		public void Execute() { }

		public void Exit()
		{
			_eventBus.Unsubscribe(EGameEvent.EndGameplay, EnterInGameMenuState);
		}

		private void EnterInGameMenuState()
		{
			_globalStateMachine.EnterIn<GameMenuState>();
		}
	}
}