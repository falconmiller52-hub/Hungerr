using Runtime.Common.Factories.StateFactory;
using Runtime.Features.DayNight.StateMachine;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.GameplayScene
{
    public class GameplayInstaller : MonoInstaller
    {
        // монобехи для инсталла
        [SerializeField] private PhaseStateMachine _phaseStateMachine;
        
        public override void InstallBindings()
        {
            BindGameplayStateMachine();
            BindPhaseStateMachine();
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