using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Features.Player.Movement;
using UnityEngine;
using Zenject;

namespace Runtime.Features.DayNight
{
    public class DayNightChangeTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameplayStateEvent _eventType;
    
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
