using FMODUnity;
using Runtime.Common.Services.Audio;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Sounds
{
    public class GameplayAmbientPlayer : MonoBehaviour
    {
        [SerializeField] private EventReference _startAmbientSound;
        [SerializeField] private EventReference _ambientTwoSound;
        [SerializeField] private float _fadeDuration = 1f;
    
        private IAudioService _audioService;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        private void Start()
        {
            _audioService.PlayAmbient(_startAmbientSound);
        }

        private void OnDisable()
        {
            _audioService.StopPlaying(_startAmbientSound);
        }

        // тест
        
        [ContextMenu("Play Second Ambient")]
        public void PlaySecondAmbient()
        {
            _audioService.PlayAmbient(_ambientTwoSound, _fadeDuration);
        }
        
        [ContextMenu("Play First Ambient")]
        public void PlayFirstAmbient()
        {
            _audioService.PlayAmbient(_startAmbientSound, _fadeDuration);
        }
    }
}
