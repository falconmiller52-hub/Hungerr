using FMODUnity;
using Runtime.Features.Player.Movement;
using UnityEngine;
using Zenject;

namespace Runtime.Common.Services.Audio.Ost
{
	public class OstTrigger : MonoBehaviour
	{
		[SerializeField] private EventReference _eventReference;

		private OstService _ostService;

		[Inject]
		private void Construct(OstService ostService)
		{
			_ostService = ostService;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<PlayerMovement>())
			{
				_ostService.StartOst(_eventReference);
			}
		}
	}
}