using PrimeTween;
using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using TMPro;
using UnityEngine;
using Zenject;

namespace Runtime.Features.DayNight.DaysCounter
{
    public class DaysCounter : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text _dayText;
        
        [Header("Fade Settings")]
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _displayDuration = 2f;
        
        private EventBus _eventBus;
        private Sequence _fadeSequence;
        
        [Inject]
        private void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Start()
        {
            if (_dayText != null) 
                _dayText.alpha = 0;
            
            _eventBus.Subscribe<EGameplayChangedStateEvent, int>(EGameplayChangedStateEvent.OnStartNightPhase, StartNightPhaseHandler);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<EGameplayChangedStateEvent, int>(EGameplayChangedStateEvent.OnStartNightPhase, StartNightPhaseHandler);
        }

        private void OnDestroy()
        {
            _fadeSequence.Stop();
        }
        
        private void StartNightPhaseHandler(int currentDay)
        {
            UpdateVisual(currentDay);
        }

        private void UpdateVisual(int currentDay)
        {
            if (_dayText == null) 
                return;
            
            _fadeSequence.Stop();
            _dayText.text = $"Night: {currentDay}";
            
            _fadeSequence = Sequence.Create()
                // Появление (Альфа от текущей до 1)
                .Chain(Tween.Alpha(_dayText, startValue: 0, endValue: 1, duration: _fadeDuration))
                // Пауза, пока текст виден
                .ChainDelay(_displayDuration)
                // Исчезновение (Альфа до 0)
                .Chain(Tween.Alpha(_dayText, endValue: 0, duration: _fadeDuration));
        }
    }
}
