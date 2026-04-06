using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Features.Interactable;
using UnityEngine;
using Zenject;

namespace Runtime.Features.DayNight
{
    /// <summary>
    /// компонент-триггер отвечающий за вызов ивента о намерении сменить время суток
    /// </summary>
    public class DayNightChangeTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private EGameplayStateEvent _eventType;
    
        private EventBus _eventBus;

        [Inject]
        private void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Interact()
        {
            _eventBus.Trigger(_eventType);
        }
    }
}
