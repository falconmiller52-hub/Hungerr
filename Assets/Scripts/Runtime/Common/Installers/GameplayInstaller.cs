using Runtime.Common.Services.Input;
using Zenject;

namespace Runtime.Common.Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InputHandler>().AsSingle();
        }
    }
}