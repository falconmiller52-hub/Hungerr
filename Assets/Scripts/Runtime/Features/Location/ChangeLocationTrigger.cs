using Runtime.Common.Services.LoadingCurtain;
using Runtime.Common.Services.Pause;
using Runtime.Features.Interactable;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Location
{
	/// <summary>
	/// Триггер смены локации, вешается на объект с коллайдером и если игрок завзаимодействует с этим объектом то сменится поз. игрока на nextPosition
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class ChangeLocationTrigger : MonoBehaviour, IInteractable
	{
		[SerializeField] private Transform _nextPositionTransform;
		[SerializeField] private LocationChangerData _locationChangerData;
		
		private LocationChanger _locationChanger;
		private ILoadingCurtain _curtain;
		private IPauseController _pauseController;

		[Inject]
		private void Construct(LocationChanger locationChanger, ILoadingCurtain loadingCurtain, IPauseController pauseController)
		{
			_locationChanger = locationChanger;
			_curtain = loadingCurtain;
			_pauseController = pauseController;
		}

		public void Interact()
		{
			_pauseController.PerformStop();
			_curtain.Show(_locationChangerData.FadeInDuration, onEnd: Teleport);
		}

		private void Teleport()
		{
			_locationChanger.ChangeLocation(_nextPositionTransform);
			_curtain.Hide(_locationChangerData.FadeOutSpeed);
			
			_pauseController.PerformResume();
		}
	}
}