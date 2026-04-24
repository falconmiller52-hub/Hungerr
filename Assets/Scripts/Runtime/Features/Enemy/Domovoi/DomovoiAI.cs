using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Common.Enums;
using Runtime.Common.Extensions;
using Runtime.Common.Services.EventBus;
using Runtime.Features.Inventory;
using Runtime.Features.Inventory.View.EntryPoint;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Enemy.Domovoi
{
    [Serializable]
    public class DomovoiLevelData
    {
        [Tooltip("Уровень (ни на что не влияет)")] 
        public int Level;
        
        [Tooltip("Максимальная сытость")] 
        public int MaxSatiety;
        
        [Tooltip("C какого дня активируется уровень")] 
        public int MinDayForLevel;
        
        [Tooltip("Насколько уменьшается голод с каждой ночью")] 
        public int DailySatietyLoss;
        
        [Tooltip("Уровень сытости при котором (и ниже) начинается активация")] 
        public int SatietyTreshholdForActivation;
        
        [Tooltip("Паттерны поведения при текущем уровне (если активация при низкой сытости есть)")] 
        public List<DomovoiPattern> Patterns;
    }
    
    public class DomovoiAI : MonoBehaviour
    {
        [SerializeField] private StorageInventory _storage; // хранилище с едой
        [SerializeField] private DomovoiLevelData[] _domovoiLevelData;

        private EventBus _eventBus;
        private DomovoiLevelData _currentLevelData;
        private InventoryWithCells _inventory;
        private int _satiety;
        private int _currentDay = 1;

        [Inject]
        private void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Start()
        {
            // если нет сохранений состояния
            if (_domovoiLevelData == null || _domovoiLevelData.Length == 0)
            {
                Debug.LogError("DomovoiAI::Start() Domovoi level data is null");
                return;
            }
            
            if (_eventBus == null)
            {
                Debug.LogError("DomovoiAI::Start() EventBus is null");
                return;
            }
            
            _currentLevelData = _domovoiLevelData[0];
            _satiety = _currentLevelData.MaxSatiety;
            
            _inventory = _storage.GetInventory();
            
            // _eventBus.Subscribe(EGameplayStateEvent.StartNightPhaseTrigger, OnStartNightPhaseTrigger);
            _eventBus.Subscribe<EGameplayChangedStateEvent, int>(EGameplayChangedStateEvent.OnStartNightPhase, OnStartNightPhaseHandler);
            _eventBus.Subscribe(EGameplayChangedStateEvent.OnEndNightPhase, OnEndNightPhaseHandler);
        }
        
        private void OnDisable()
        {
            _eventBus.Unsubscribe<EGameplayChangedStateEvent, int>(EGameplayChangedStateEvent.OnStartNightPhase, OnStartNightPhaseHandler);
            _eventBus.Unsubscribe(EGameplayChangedStateEvent.OnEndNightPhase, OnEndNightPhaseHandler);
        }
        
        private void OnEndNightPhaseHandler()
        {
            // выбираем рандомный паттерн
            var pattern = _currentLevelData.Patterns.Random();
            
            pattern.Trigger();
        }

        private void OnStartNightPhaseHandler(int currentDay)
        {
            // проверяем еду в сундуке
            // обновляем голод
            // проверяем день и переходим на новый уровень если надо
            // выбираем паттерн
            // кидает ивент о уровне сытости

            if (_inventory == null)
                _inventory = _storage.GetInventory();

            _currentDay += 1;

            foreach (DomovoiLevelData levelData in _domovoiLevelData)
            {
                if (_currentDay >= levelData.MinDayForLevel && levelData != _currentLevelData)
                    _currentLevelData = levelData;
            }
            
            List<FoodInventoryItemData> foodItems = _inventory.GetItems<FoodInventoryItemData>();
            int totalSatietyFromInventory = foodItems.Sum(foodItem => foodItem.Satiety); // считаем всю сытость от еды в сундуке

            _satiety += Math.Min(totalSatietyFromInventory, 0); // добавляем либо еду либо 0
            _satiety -= _currentLevelData.DailySatietyLoss; // отнимаем ежедвненую потерю
            
            // кидаем ивент в зависимости от уровня сытости, если мало (по текущему уровню) то будет больно днем
            if (_satiety < _currentLevelData.SatietyTreshholdForActivation)
                _eventBus.Trigger(EDomovoiSatietyLevel.Critical);
            else
                _eventBus.Trigger(EDomovoiSatietyLevel.Normal);
        }
    }
}
