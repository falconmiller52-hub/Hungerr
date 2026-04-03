using Runtime.Common.Services.Audio;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Features.Sounds;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.App
{
    public class GamePrefabsInstaller : MonoInstaller
    {
        [SerializeField] private Curtain _loadingCurtainPrefab;
        [SerializeField] private SoundEmitter _emitterPrefab;

        public override void InstallBindings()
        {
            BindLoadingCurtain();
            BindAudioService();
        }
        
        private void BindLoadingCurtain()
        {
            Container
                .BindInterfacesAndSelfTo<Curtain>()
                .FromComponentInNewPrefab(_loadingCurtainPrefab)
                .AsSingle();
        }
        
        public void BindAudioService()
        {
            // Создаем пул объектов для излучателей звука
            Container.BindMemoryPool<SoundEmitter, SoundEmitter.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_emitterPrefab)
                .UnderTransformGroup("AudioPool");
            
            Container.BindInterfacesAndSelfTo<AudioService>().AsSingle();
        }
    }
}
