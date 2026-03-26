using Runtime.Common.Services.Input;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Common.Services.StateMachine;
using Zenject;

namespace Runtime.Infra.GameplayScene.GameplayStateMachine.States
{
	/// <summary>
	/// Entry point for the gameplay state; initializes scene components and transitions to Play state.
	/// </summary>
	public class EntryGameplayState : IState
	{
		private readonly SceneStateMachine _sceneStateMachine;
		private readonly ILoadingCurtain _loadingCurtain;
		private readonly InputHandler _inputHandler;

		[Inject]
		public EntryGameplayState(SceneStateMachine sceneStateMachine, ILoadingCurtain loadingCurtain, InputHandler inputHandler)
		{
			_sceneStateMachine = sceneStateMachine;
			_loadingCurtain = loadingCurtain;
			_inputHandler = inputHandler;
		}

		public void Enter()
		{
			// init services
			_inputHandler.Enable();
			_loadingCurtain.Hide();
			
			_sceneStateMachine.EnterIn<PlayGameplayState>();
		}

		public void Exit()
		{
		}
	}
}