using Runtime.Common.Services.ItemsIdentifier;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.App
{
	public class GlobalMonoInstaller : MonoInstaller
	{
		[SerializeField] private ItemsIdentifierSO _itemsIdentifierSO;
		
		public override void InstallBindings()
		{
			Container.BindInstance(_itemsIdentifierSO);
		}
	}
}