using System;
using Runtime.Infra.App.GlobalStateMachine;
using Runtime.Infra.App.GlobalStateMachine.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.Features.GameOver.View
{
	public class GameOverCurtain : MonoBehaviour, IGameOverCurtain
	{
		[SerializeField] private GameObject _gameOverPanel;
		[SerializeField] private Button _backToMenu;

		private GlobalStateMachine _globalStateMachine;

		[Inject]
		private void Construct(GlobalStateMachine globalStateMachine)
		{
			_globalStateMachine = globalStateMachine;
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
			_globalStateMachine.EnterIn<GameMenuState>();
		}
	}
}