using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Features.Player.Movement;
using UnityEngine;
using Zenject;

namespace Runtime.Features.DayNight
{
    public class DayNightChangeTrigger : MonoBehaviour
    {
        [SerializeField] private GameplayStateEvent _eventType;
    
        private EventBus _eventBus;

        [Inject]
        private void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerMovement>() != null)
                _eventBus.Trigger(_eventType);
        }
    }
}
