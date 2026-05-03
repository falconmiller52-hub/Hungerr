using FMODUnity;
using Runtime.Common.Enums;
using Runtime.Common.Services.Audio.Ost;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Pause;
using Runtime.Features.GameOver.View;
using UnityEngine;

namespace Runtime.Features.GameOver
{
	public class GameOverTriggerHandler
	{
		private readonly EventBus _eventBus;
		private readonly OstService _ostService;
		private readonly IPauseController _pauseController;
		private readonly IGameOverCurtain _gameOverCurtain;

		// TODO: Заменить в будущем на SoProvider
		private readonly string _pathToTheEndTheme;

		public GameOverTriggerHandler(EventBus eventBus, IPauseController pauseController,
						IGameOverCurtain gameOverCurtain, OstService ostService)
		{
			_eventBus = eventBus;
			_pauseController = pauseController;
			_gameOverCurtain = gameOverCurtain;
			_ostService	= ostService;

			_pathToTheEndTheme = "event:/OST/The End (4)";
			
			_eventBus.Subscribe(EGameOver.PlayerOnZeroHealth, PlayerZeroHealthGameOver);
		}

		private void PlayerZeroHealthGameOver()
		{
			_eventBus.Unsubscribe(EGameOver.PlayerOnZeroHealth, PlayerZeroHealthGameOver);

			_gameOverCurtain.ShowCurtain();
			GameOver();
		}

		private void GameOver()
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			_pauseController.PerformStop();
			
			_ostService.StartOst(RuntimeManager.PathToEventReference(_pathToTheEndTheme));
		}
	}
}