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
		readonly ILoadingCurtain _loadingCurtain;

		[Inject]
		public GameBootstrapState(GlobalStateMachine globalStateMachine, ILoadingCurtain loadingCurtain)
		{
			_globalStateMachine = globalStateMachine;
			_loadingCurtain = loadingCurtain;
		}

		public void Enter()
		{
			Debug.Log("Boostrap State");

			// устанавливает глобальные настройки
			Application.targetFrameRate = 60;

			// инитит и включает глобальные сервисы
			_loadingCurtain.Show();

			_globalStateMachine.EnterIn<GameMenuState>();
		}

		public void Execute() { }

		public void Exit() { }
	}
}