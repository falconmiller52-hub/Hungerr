using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Runtime.Common.Enums;
using Runtime.Common.Extensions;
using Runtime.Common.Services.EventBus;
using Runtime.Features.Inventory;
using Runtime.Features.Inventory.View.EntryPoint;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Enemy.Domovoi
{
    public class DomovoiAI : MonoBehaviour
    {
        [SerializeField] private StorageInventory _storage; // хранилище с едой
        [SerializeField] private DomovoiLevelData[] _domovoiLevelData;

        [SerializeField, ReadOnly] private int _satiety;
        [SerializeField, ReadOnly] private DomovoiLevelData _currentLevelData;
        private EventBus _eventBus;
        private InventoryWithCells _inventory;

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
            
            // кидаем ивент в зависимости от уровня сытости, если мало (по текущему уровню) то будет больно
            if (_satiety < _currentLevelData.SatietyTreshholdForActivation)
                _eventBus.Trigger(EDomovoiSatietyLevel.Critical);
            else
                _eventBus.Trigger(EDomovoiSatietyLevel.Normal);
        }

        private void OnStartNightPhaseHandler(int currentDay)
        {
            if (_inventory == null)
                _inventory = _storage.GetInventory();

            // проверяем день и переходим на некст уровень если надо
            foreach (DomovoiLevelData levelData in _domovoiLevelData)
            {
                if (currentDay >= levelData.MinDayForLevel && levelData != _currentLevelData)
                    _currentLevelData = levelData;
            }
            
            List<FoodInventoryItemData> foodItems = _inventory.GetItems<FoodInventoryItemData>();
            int totalSatietyFromInventory = foodItems.Sum(foodItem => foodItem.Satiety); // считаем всю сытость от еды в сундуке

            int delta = Math.Max(totalSatietyFromInventory, 0) - Math.Max(_currentLevelData.DailySatietyLoss, 0);
            _satiety = Math.Max(_satiety + delta, 0);

            _inventory.RemoveAllItemsByType<FoodInventoryItemData>();
        }
    }
}
