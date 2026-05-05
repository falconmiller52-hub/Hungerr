using FMODUnity;
using Runtime.Common.Constants;
using Runtime.Common.Enums;
using Runtime.Common.Services.Audio.Ost;
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
		private readonly EventBus _eventBus;
		private readonly ILoadingCurtain _loadingCurtain;
		private readonly OstService _ostService;
		private readonly GlobalStateMachine _stateMachine;
		// TODO : В данный момент захардкодил путь к ивенту, потом переделать на SoProvider
		private readonly string _pathToMainMenuTheme;

		[Inject]
		public GameMenuState(ILoadingCurtain loadingCurtain, EventBus eventBus, GlobalStateMachine stateMachine,
						OstService ostService)
		{
			_loadingCurtain = loadingCurtain;
			_eventBus = eventBus;
			_stateMachine = stateMachine;
			_ostService = ostService;

			_pathToMainMenuTheme = "event:/OST/Main Menu Theme";
		}

		public void Enter()
		{
			Debug.Log("Menu  State");
			
			SceneManager.LoadScene(Scenes.MenuName);
			_loadingCurtain.Hide();
			
			_ostService.StartOst(RuntimeManager.PathToEventReference(_pathToMainMenuTheme));

			_eventBus.Subscribe(EGameEvent.StartGameplay, GoToGameplay);
			_eventBus.Subscribe(EGameEvent.QuitGame, ExitMenu);
		}

		public void Execute() { }

		public void Exit()
		{
			Debug.Log("Exit Menu  State");

			_eventBus.Unsubscribe(EGameEvent.StartGameplay, GoToGameplay);
			_eventBus.Unsubscribe(EGameEvent.QuitGame, ExitMenu);
		}

		void GoToGameplay()
		{
			_loadingCurtain.Show(onEnd: () =>
			{
				SceneManager.LoadScene(Scenes.GameplayScene);
			
				_stateMachine.EnterIn<GameplayLoopState>();
			});
		}

		void ExitMenu()
		{
			_stateMachine.EnterIn<GameExitState>();
		}
	}
}