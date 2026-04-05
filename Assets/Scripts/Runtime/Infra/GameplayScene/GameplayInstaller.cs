using Runtime.Common.Factories.StateFactory;
using Runtime.Features.DayNight.StateMachine;
using Runtime.Features.Enemy;
using Runtime.Features.Location;
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

		public override void InstallBindings()
		{
			BindGameplayStateMachine();
			BindPhaseStateMachine();
			BindLocationChanger();
			BindEnemiesController();
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

		private void BindGameplayStateMachine()
		{
			Container.Bind<StateFactory>().AsSingle();
			Container.Bind<GameplayStateMachine.SceneStateMachine>().AsSingle();
		}
	}
}