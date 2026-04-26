using FMODUnity;
using Runtime.Common.Services.Audio;
using Runtime.Common.Services.Audio.Ost;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Sounds
{
	public class GameplayAmbientPlayer : MonoBehaviour
	{
		[SerializeField] private EventReference _startAmbientSound;

		private OstService _ostService;

		[Inject]
		private void Construct(OstService ostService)
		{
			_ostService = ostService;
		}

		private void Start()
		{
			_ostService.StartOst(_startAmbientSound);
		}
	}
}