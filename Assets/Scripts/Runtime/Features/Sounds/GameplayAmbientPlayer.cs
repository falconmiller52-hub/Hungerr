using FMODUnity;
using Runtime.Common.Services.Audio;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Sounds
{
	public class GameplayAmbientPlayer : MonoBehaviour
	{
		[SerializeField] private EventReference _startAmbientSound;
		[SerializeField] private float _fadeDuration = 1f;

		private IAudioService _audioService;

		[Inject]
		private void Construct(IAudioService audioService)
		{
			_audioService = audioService;
		}

		private void Start()
		{
			_audioService.PlaySound(_startAmbientSound);
		}
	}
}