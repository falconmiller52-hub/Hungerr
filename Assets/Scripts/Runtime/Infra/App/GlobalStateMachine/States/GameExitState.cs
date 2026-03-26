using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.StateMachine;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.App.GlobalStateMachine.States
{
	/// <summary>
	///     Exit state: clean shutdown and prepare app termination.
	/// </summary>
	public class GameExitState : IState
	{
		readonly EventBus _eventBus;

		[Inject]
		public GameExitState(EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		public void Enter()
		{
			Debug.Log("Exit  State");

			// здесь корректно выгружаем ресурсы
			_eventBus.Dispose();

			Application.Quit();
		}

		public void Exit()
		{
		}
	}
}