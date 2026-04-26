using FMODUnity;
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
			if (other.CompareTag("Player"))
			{
				_ostService.StartOst(_eventReference);
			}
		}
	}
}