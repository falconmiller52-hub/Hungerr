using Runtime.Common.Factories.StateFactory;
using Runtime.Common.Services.Audio;
using Runtime.Common.Services.Audio.Sound;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.ResourceLoad;
using Zenject;

namespace Runtime.Infra.App
{
	public class GlobalInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			BindInputService();
			BindGameStateMachine();
			BindEventBus();
			BindGlobalEntryPoint();
			BindResourceLoader();
			BindAudio();
		}

		private void BindResourceLoader()
		{
			Container.BindInterfacesAndSelfTo<ResourceLoader>().AsSingle();
		}

		private void BindInputService() =>
						Container.BindInterfacesAndSelfTo<InputHandler>().AsSingle();

		private void BindGameStateMachine()
		{
			Container.Bind<StateFactory>().AsSingle();
			Container.Bind<GlobalStateMachine.GlobalStateMachine>().AsSingle();
		}

		private void BindGlobalEntryPoint() =>
						Container.BindInterfacesAndSelfTo<GlobalEntryPoint>().AsSingle();

		private void BindEventBus() =>
						Container.Bind<EventBus>().AsSingle();

		public void BindAudio()
		{
			Container.BindInterfacesAndSelfTo<SoundService>().AsSingle();
		}
	}
}