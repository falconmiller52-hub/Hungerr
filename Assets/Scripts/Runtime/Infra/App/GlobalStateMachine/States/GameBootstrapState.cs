using Runtime.Common.Services.Input;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Common.Services.StateMachine;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.App.GlobalStateMachine.States
{
	/// <summary>
	///     Bootstrap state: sets up initial services and transitions to the main menu.
	/// </summary>
	public class GameBootstrapState : IState
	{
		readonly GlobalStateMachine _globalStateMachine;
		readonly InputHandler _inputHandler;
		readonly ILoadingCurtain _loadingCurtain;

		[Inject]
		public GameBootstrapState(GlobalStateMachine globalStateMachine, InputHandler inputHandler, ILoadingCurtain loadingCurtain)
		{
			_globalStateMachine = globalStateMachine;
			_inputHandler = inputHandler;
			_loadingCurtain = loadingCurtain;
		}

		public void Enter()
		{
			Debug.Log("Boostrap State");

			Application.targetFrameRate = 60;

			// init services 
			_loadingCurtain.Show();

			_globalStateMachine.EnterIn<GameMenuState>();
		}

		public void Exit()
		{
		}
	}
}