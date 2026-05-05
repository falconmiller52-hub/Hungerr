using FMODUnity;
using Runtime.Common.Enums;
using Runtime.Common.Services.Audio.Ost;
using Runtime.Common.Services.EventBus;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Sounds
{
	public class GameplayAmbientPlayer : MonoBehaviour
	{
		[SerializeField] private EventReference _startAmbientSound;

		private OstService _ostService;
		private EventBus _eventBus;

		[Inject]
		private void Construct(OstService ostService, EventBus eventBus)
		{
			_ostService = ostService;
			_eventBus = eventBus;
		}

		private void Start()
		{
            if (_eventBus == null)
            {
                Debug.LogError("GameplayAmbientPlayer::Start() EventBus is null");
                return;
            }
            _eventBus.Subscribe(EDomovoiSatietyLevel.Normal, PlayStandartOst);
            _eventBus.Subscribe(EGameplayChangePhaseTriggerEvent.StartNightTrigger, PlayStreetOst);
			PlayStandartOst();
        }

		private void PlayStandartOst()
		{
            _ostService.StartOst(_startAmbientSound);
        }
        private void PlayStreetOst()
        {
			Debug.Log(12345678);
            _ostService.StartOst(_startAmbientSound);
        }
    }
}