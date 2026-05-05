using Runtime.Common.Enums;
using Runtime.Common.Helpers;
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
		[SerializeField] private EGameplayChangePhaseTriggerEvent _triggerEventType;

		private EventBus _eventBus;

		[Inject]
		private void Construct(EventBus eventBus)
		{
			_eventBus = eventBus;	
		}

		public void Interact()
		{
			switch (_triggerEventType)
			{
				case EGameplayChangePhaseTriggerEvent.StartDayTrigger:
					var data = new StartDayTriggerEventData();
					data.ForceNightEnd = false;

					_eventBus.Trigger(_triggerEventType, data);
					break;

				case EGameplayChangePhaseTriggerEvent.StartNightTrigger:
					_eventBus.Trigger(_triggerEventType);
					_eventBus.Trigger(EGameplayChangePhaseTriggerEvent.StartNightTrigger);

					break;
			}
		}
	}
}