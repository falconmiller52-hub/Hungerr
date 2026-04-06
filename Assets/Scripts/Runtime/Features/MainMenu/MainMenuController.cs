using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.Features.MainMenu
{
	public class MainMenuController : MonoBehaviour
	{
		[SerializeField] private Button _startGameButton;
		[SerializeField] private Button _exitGameButton;
		[SerializeField] private string _gameplaySceneName;

		private EventBus _eventBus;

		[Inject]
		private void Construct(EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		private void OnEnable()
		{
			_startGameButton.onClick.AddListener(HandleStartGame);
			_exitGameButton.onClick.AddListener(HandleExitGame);
		}

		private void OnDisable()
		{
			_startGameButton.onClick.RemoveListener(HandleStartGame);
			_exitGameButton.onClick.RemoveListener(HandleExitGame);
		}

		private void HandleExitGame()
		{
			_eventBus.Trigger(EGameEvent.QuitGame);
		}

		private void HandleStartGame()
		{
			_eventBus.Trigger(EGameEvent.StartGameplay);
		}
	}
}