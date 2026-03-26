using Runtime.Features.DayNight;
using UnityEngine;

namespace Runtime.Features.GameplayDirector
{
    public class GameplayDirector : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private DayCycleVisualChanger _visualChanger;
    
        [Header("Time Settings")]
        [SerializeField, Tooltip("Длительность полных суток в секундах"), Range(1, 3600)]
        private float _fullDayDuration = 120f; 
    
        [SerializeField, Range(0, 1)] 
        private float _timeProgress = 0f;

        [Header("Survival Settings"), Tooltip("Сколько секунд можно быть на улице ночью")]
        [SerializeField] private float _maxNightTimeOutside = 30f;
        private float _survivalTimer;

        // Свойства для других скриптов
        public bool IsPlayerInside { get; set; }
        public float TimeProgress => _timeProgress;

        // Условные границы ночи (например, с 18:00 до 06:00 это прогресс 0.75 -> 1.0 и 0.0 -> 0.25)
        public bool IsNight => _timeProgress > 0.7f || _timeProgress < 0.2f;

        private void Update()
        {
            _timeProgress += Time.deltaTime / _fullDayDuration;
            
            if (_timeProgress >= 1f) 
                _timeProgress = 0f;
            
            _visualChanger.UpdateDayCycle(_timeProgress);
            
            HandleSurvival();
        }

        private void HandleSurvival()
        {
            if (IsNight && !IsPlayerInside)
            {
                _survivalTimer += Time.deltaTime;
                Debug.Log($"Игрок на улице ночью! Осталось: {_maxNightTimeOutside - _survivalTimer:F1} сек.");

                if (_survivalTimer >= _maxNightTimeOutside)
                {
                    ApplyNightPenalty();
                }
            }
            else
            {
                // Если настал день или зашли в дом — постепенно восстанавливаем таймер
                _survivalTimer = Mathf.MoveTowards(_survivalTimer, 0, Time.deltaTime * 0.5f);
            }
        }

        private void ApplyNightPenalty()
        {
            Debug.LogError("Игрок погиб или замерз!");
        }
    }
}
