using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.ResourceLoad;
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
		private readonly IResourceLoader _resourceLoader;

		[Inject]
		public GameExitState(EventBus eventBus, IResourceLoader resourceLoader)
		{
			_eventBus = eventBus;
			_resourceLoader = resourceLoader;
		}

		public void Enter()
		{
			Debug.Log("Exit  State");

			// здесь корректно выгружаем ресурсы
			_eventBus.Dispose();
			_resourceLoader.UnloadUnused();

			Application.Quit();
		}

		public void Execute() { }

		public void Exit() {}
	}
}