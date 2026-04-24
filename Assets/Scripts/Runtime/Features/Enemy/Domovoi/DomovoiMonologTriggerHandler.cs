using System;
using FMODUnity;
using Ink.Runtime;
using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Features.Dialog;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Enemy.Domovoi
{
    public class DomovoiMonologTriggerHandler : MonoBehaviour
    {
        [SerializeField] private TextAsset _criticalSatietyLevelMonologText;
        [SerializeField] private TextAsset _normalSatietyLevelMonologText;
        
        [SerializeField, Tooltip("Звук игрока при монологе")] 
        private EventReference _playerMonologSound;
        
        private EventBus _eventBus;
        private DialogSystem _dialogSystem;

        [Inject]
        private void Construct(EventBus eventBus, DialogSystem dialogSystem)
        {
            _eventBus = eventBus;
            _dialogSystem = dialogSystem;
        }

        private void Start()
        {
            _eventBus.Subscribe(EDomovoiSatietyLevel.Critical, OnCriticalDomovoiSatietyLevelTriggered);
            _eventBus.Subscribe(EDomovoiSatietyLevel.Normal, OnNormalDomovoiSatietyLevelTriggered);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe(EDomovoiSatietyLevel.Critical, OnCriticalDomovoiSatietyLevelTriggered);
            _eventBus.Unsubscribe(EDomovoiSatietyLevel.Normal, OnNormalDomovoiSatietyLevelTriggered);
        }
        
        private void OnNormalDomovoiSatietyLevelTriggered()
        {
            if (_normalSatietyLevelMonologText == null)
                return;
            
            Story monolog = new Story(_normalSatietyLevelMonologText.text);

            _dialogSystem.StartStory(monolog, _playerMonologSound, isMonolog: true);
        }
        
        private void OnCriticalDomovoiSatietyLevelTriggered()
        {
            if (_criticalSatietyLevelMonologText == null)
                return;
            
            Story monolog = new Story(_criticalSatietyLevelMonologText.text);

            _dialogSystem.StartStory(monolog, _playerMonologSound, isMonolog: true);
        }
        
    }
}
