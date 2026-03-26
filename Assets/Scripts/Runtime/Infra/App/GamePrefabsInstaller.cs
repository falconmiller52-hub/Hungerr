using Runtime.Common.Services.LoadingCurtain;
using UnityEngine;
using Zenject;

namespace DevFuckers._Project.CodeBase.Runtime.Infrastructure.GameApp.EntryPoint
{
    public class GamePrefabsInstaller : MonoInstaller
    {
        [SerializeField] private Curtain _loadingCurtain;

        public override void InstallBindings()
        {
            BindLoadingCurtain();
        }
        
        private void BindLoadingCurtain()
        {
            Container
                .BindInterfacesAndSelfTo<Curtain>()
                .FromComponentInNewPrefab(_loadingCurtain)
                .AsSingle();
        }
    }
}
