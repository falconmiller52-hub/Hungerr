using Runtime.Infra.App.GlobalStateMachine;
using Runtime.Infra.GameplayScene.GameplayStateMachine;
using Runtime.Infra.GameplayScene.GameplayStateMachine.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.Features.GameOver.View
{
	public class GameOverCurtain : MonoBehaviour, IGameOverCurtain
	{
		[SerializeField] private GameObject _gameOverPanel;
		[SerializeField] private Button _backToMenu;

		private SceneStateMachine _sceneStateMachine;

		[Inject]
		private void Construct(SceneStateMachine sceneStateMachine)
		{
			_sceneStateMachine = sceneStateMachine;
		}

		private void OnEnable()
		{
			_backToMenu.onClick.AddListener(BackToMenu);
		}

		private void OnDisable()
		{
			_backToMenu.onClick.RemoveListener(BackToMenu);
		}

		public void ShowCurtain()
		{
			_gameOverPanel.SetActive(true);
		}
		
		private void BackToMenu()
		{
			_sceneStateMachine.EnterIn<ExitGameplayState>();
		}
	}
}