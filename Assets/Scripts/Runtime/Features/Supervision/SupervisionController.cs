using Runtime.Common.Services.EventBus;
using Runtime.Features.Location;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Supervision
{
	public class SupervisionController : MonoBehaviour
	{
		[SerializeField] private Transform _spawnPointForPlayerAfterDontFeedPunishment;

		private EventBus _eventBus;
		private LocationChanger _locationChanger;

		[Inject]
		private void Construct(EventBus eventBus, LocationChanger locationChanger)
		{
			_eventBus = eventBus;
			_locationChanger = locationChanger;
		}

		public void OnDomovoiDontFed()
		{
			// хендлим то что домовой не был покормлен
			Debug.Log("Domovoi not fed");
		}

		public void OnLateAtNight()
		{
			Debug.Log("late at night");
		}
	}
}