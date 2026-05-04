using Runtime.Common.Enums;
using Runtime.Common.Helpers;
using Runtime.Common.Services.EventBus;
using UnityEngine;
using Zenject;

namespace Runtime.Features.DayNight.DaysCounter
{
	public class CurrentDayController : MonoBehaviour
	{
		[SerializeField] private DaysCounter _dayCounter;

		private EventBus _eventBus;
		private int _currentDay;

		public int CurrentDay => _currentDay;

		[Inject]
		private void Construct(EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		public void Init(int startDay)
		{
			_currentDay = startDay;
			_eventBus.Subscribe<EGameplayChangedPhaseEvent, StartNightEventData>(EGameplayChangedPhaseEvent.NightStarted, StartNightPhaseHandler);
		}

		private void OnDisable()
		{
			_eventBus.Unsubscribe<EGameplayChangedPhaseEvent, StartNightEventData>(EGameplayChangedPhaseEvent.NightStarted, StartNightPhaseHandler);
		}

		private void StartNightPhaseHandler(StartNightEventData data)
		{
			_currentDay = data.CurrentDay;
			_dayCounter.UpdateVisual(_currentDay);
		}
	}
}