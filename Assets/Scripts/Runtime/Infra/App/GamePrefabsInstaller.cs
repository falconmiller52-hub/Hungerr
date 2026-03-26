using Runtime.Common.Services.LoadingCurtain;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.App
{
    public class GamePrefabsInstaller : MonoInstaller
    {
        [SerializeField] private Curtain _loadingCurtainPrefab;

        public override void InstallBindings()
        {
            BindLoadingCurtain();
        }
        
        private void BindLoadingCurtain()
        {
            Container
                .BindInterfacesAndSelfTo<Curtain>()
                .FromComponentInNewPrefab(_loadingCurtainPrefab)
                .AsSingle();
        }
    }
}
