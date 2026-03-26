using System;
using System.Collections.Generic;
using Runtime.Common.Enums;

namespace Runtime.Common.Services.EventBus
{
    /// <summary>
    ///     Lightweight event bus used to publish and subscribe to global events.
    /// </summary>
    public class EventBus : IDisposable
	{
		readonly Dictionary<Event, Action> _events = new();

        /// <summary>
        ///     Clears all registered event handlers.
        /// </summary>
        public void Dispose()
		{
			_events.Clear();
		}

        /// <summary>
        ///     Subscribes an action to the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event to subscribe to.</param>
        /// <param name="action">Action to invoke when the event is triggered.</param>
        public void Subscribe(Event eventType, Action action)
		{
			_events.TryAdd(eventType, delegate { });

			_events[eventType] += action;
		}

        /// <summary>
        ///     Unsubscribes an action from the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event to unsubscribe from.</param>
        /// <param name="action">Action to remove.</param>
        public void Unsubscribe(Event eventType, Action action)
		{
			if (_events.ContainsKey(eventType))
				_events[eventType] -= action;
		}

        /// <summary>
        ///     Triggers the specified event, invoking all subscribed actions if any.
        /// </summary>
        /// <param name="eventType">Event to trigger.</param>
        public void Trigger(Event eventType)
		{
			// Используем TryGetValue для безопасности
			if (_events.TryGetValue(eventType, out var action))
				action?.Invoke();
		}
	}
}