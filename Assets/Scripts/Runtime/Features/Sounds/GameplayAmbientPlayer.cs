using Runtime.Common.Services.Audio;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Sounds
{
    public class GameplayAmbientPlayer : MonoBehaviour
    {
        [SerializeField] private SoundData _startAmbientSound;
    
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
    }
}
