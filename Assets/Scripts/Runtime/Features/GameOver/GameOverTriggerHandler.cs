using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Pause;
using Runtime.Features.GameOver.View;
using UnityEngine;

namespace Runtime.Features.GameOver
{
	public class GameOverTriggerHandler
	{
		private readonly EventBus _eventBus;
		private readonly IPauseController _pauseController;
		private readonly IGameOverCurtain _gameOverCurtain;

		public GameOverTriggerHandler(EventBus eventBus, IPauseController pauseController,
						IGameOverCurtain gameOverCurtain)
		{
			_eventBus = eventBus;
			_pauseController = pauseController;
			_gameOverCurtain = gameOverCurtain;

			_eventBus.Subscribe(EGameOver.PlayerOnZeroHealth, PlayerZeroHealthGameOver);
		}
		
		private void PlayerZeroHealthGameOver()
		{
			_gameOverCurtain.ShowCurtain();
			GameOver();
		}

		private void GameOver()
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			
			_pauseController.PerformStop();
		}
	}
}