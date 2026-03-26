using Runtime.Common.Factories.StateFactory;
using Zenject;

namespace Runtime.Infra.GameplayScene
{
    public class GameplayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindGameplayStateMachine();
        }
        
        private void BindGameplayStateMachine()
        {
            Container.Bind<StateFactory>().AsSingle();
            Container.Bind<GameplayStateMachine.SceneStateMachine>().AsSingle();
        }
    }
}