using Runtime.Common.Services.Audio;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Features.GameOver.View;
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