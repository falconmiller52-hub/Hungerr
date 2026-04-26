using Runtime.Common.Factories.StateFactory;
using Runtime.Common.Services.Pause;
using Runtime.Features._Story;
using Runtime.Features.DayNight.StateMachine;
using Runtime.Features.Enemy;
using Runtime.Features.GameOver.View;
using Runtime.Features.ItemSpawner;
using Runtime.Features.Location;
using Runtime.Features.Trade;
using UnityEngine;
using Zenject;	

namespace Runtime.Infra.GameplayScene
{
	public class GameplayInstaller : MonoInstaller
	{
		// монобехи для инсталла
		[SerializeField] private PhaseStateMachine _phaseStateMachine;
		[SerializeField] private LocationChanger _locationChanger;
		[SerializeField] private EnemiesBootstrap _enemiesBootstrap;
		[SerializeField] private StorySystem _storySystem;
		[SerializeField] private ItemSpawner _itemSpawner;
		[SerializeField] private GameOverCurtain _gameOverCurtainPrefab;

		public override void InstallBindings()
		{
			BindGameplayStateMachine();
			BindPhaseStateMachine();
			BindLocationChanger();
			BindEnemiesController();
			BindStorySystem();
			BindPauseController();
			BindItemSpawner();
			BindTradeSystem();
			BindGameOver();
		}

		private void BindGameOver()
		{
			Container.Bind<IGameOverCurtain>().To<GameOverCurtain>().FromComponentInNewPrefab(_gameOverCurtainPrefab)
							.AsSingle();
		}

		private void BindTradeSystem()
		{
			Container.Bind<ITrade>().To<Trade>().AsSingle();
			Container.Bind<TradeTagHandler>().AsSingle();
		}

		private void BindPauseController()
		{
			Container.Bind<IPauseController>().To<PauseController>().AsSingle();
		}

		private void BindStorySystem()
		{
			Container.Bind<StorySystem>().FromInstance(_storySystem).AsSingle();
			Container.Bind<StoryTagSystem>().AsSingle();
		}

		private void BindEnemiesController()
		{
			Container.Bind<EnemiesBootstrap>().FromInstance(_enemiesBootstrap).AsSingle();
		}

		private void BindLocationChanger()
		{
			Container.Bind<LocationChanger>().FromInstance(_locationChanger).AsSingle();
		}

		private void BindPhaseStateMachine()
		{
			Container.Bind<PhaseStateMachine>().FromInstance(_phaseStateMachine).AsSingle();
		}

		private void BindItemSpawner()
		{
			Container.Bind<ItemSpawner>().FromInstance(_itemSpawner).AsSingle();
		}

		private void BindGameplayStateMachine()
		{
			Container.Bind<StateFactory>().AsSingle();
			Container.Bind<GameplayStateMachine.SceneStateMachine>().AsSingle();
		}
	}
}