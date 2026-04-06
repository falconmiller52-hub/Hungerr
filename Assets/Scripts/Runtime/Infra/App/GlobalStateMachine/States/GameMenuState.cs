using Runtime.Common.Constants;
using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Common.Services.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Runtime.Infra.App.GlobalStateMachine.States
{
    /// <summary>
    ///     Menu state responsible for loading the main menu scene and handling menu events.
    /// </summary>
    public class GameMenuState : IState
	{
		readonly EventBus _eventBus;
		readonly ILoadingCurtain _loadingCurtain;
		readonly GlobalStateMachine _stateMachine;

		[Inject]
		public GameMenuState(ILoadingCurtain loadingCurtain, EventBus eventBus, GlobalStateMachine stateMachine)
		{
			_loadingCurtain = loadingCurtain;
			_eventBus = eventBus;
			_stateMachine = stateMachine;
		}

		public void Enter()
		{
			Debug.Log("Menu  State");

			SceneManager.LoadScene(Scenes.MenuName);
			_loadingCurtain.Hide();

			_eventBus.Subscribe(EGameEvent.StartGameplay, GoToGameplay);
			_eventBus.Subscribe(EGameEvent.QuitGame, ExitMenu);
		}

		public void Exit()
		{
			Debug.Log("Exit Menu  State");

			_eventBus.Unsubscribe(EGameEvent.StartGameplay, GoToGameplay);
			_eventBus.Unsubscribe(EGameEvent.QuitGame, ExitMenu);
		}

		void GoToGameplay()
		{
			_loadingCurtain.Show();
			
			SceneManager.LoadScene(Scenes.GameplayScene);
			
			_stateMachine.EnterIn<GameplayLoopState>();
		}

		void ExitMenu()
		{
			_stateMachine.EnterIn<GameExitState>();
		}
	}
}