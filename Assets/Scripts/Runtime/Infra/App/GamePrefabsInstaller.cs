using Runtime.Common.Services.Audio;
using Runtime.Common.Services.LoadingCurtain;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.App
{
	public class GamePrefabsInstaller : MonoInstaller
	{
		[SerializeField] private Curtain _loadingCurtainPrefab;
		[SerializeField] private OstService _ostService;

		public override void InstallBindings()
		{
			BindLoadingCurtain();

			BindOst();
		}

		private void BindOst()
		{
			Container.Bind<OstService>().FromComponentInNewPrefab(_ostService).AsSingle();
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