using Runtime.Common.Factories.StateFactory;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.StateMachine;
using Runtime.Common.Services.Updateable;
using Runtime.Features.DayNight.StateMachine;
using Zenject;

namespace Runtime.Infra.GameplayScene.GameplayStateMachine.States
{
	/// <summary>
	/// Entry point for the gameplay state; initializes scene components and transitions to Play state.
	/// </summary>
	public class EntryGameplayState : IState
	{
		private readonly SceneStateMachine _sceneStateMachine;
		private readonly PhaseStateMachine _phaseStateMachine;
		private readonly StateFactory _stateFactory;
		private readonly InputHandler _inputHandler;

		[Inject]
		public EntryGameplayState(SceneStateMachine sceneStateMachine, PhaseStateMachine phaseStateMachine, StateFactory stateFactory, InputHandler inputHandler)
		{
			_sceneStateMachine = sceneStateMachine;
			_phaseStateMachine = phaseStateMachine;
			_stateFactory = stateFactory;
			_inputHandler = inputHandler;
		}

		public void Enter()
		{
			_inputHandler.Init();
			// init PhaseStateMachine
			DayPhaseState dayPhaseState = _stateFactory.Create<DayPhaseState>();
			_phaseStateMachine.RegisterState(dayPhaseState);
			
			NightPhaseState nightPhaseState = _stateFactory.Create<NightPhaseState>();
			_phaseStateMachine.RegisterState(nightPhaseState);
			
			_phaseStateMachine.EnterIn<DayPhaseState>();
			
			// Enter in main Gameplay State
			_sceneStateMachine.EnterIn<PlayGameplayState>();
		}

	
		public void Exit()
		{
		}
	}
}