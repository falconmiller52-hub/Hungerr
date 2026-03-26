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
		private readonly Dictionary<(Type, object), Action> _events = new();

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
        public void Subscribe<T>(T eventType, Action action) where T : Enum
        {
	        var key = (typeof(T), (object)eventType);
            
	        if (!_events.ContainsKey(key))
		        _events[key] = delegate { };

	        _events[key] += action;
        }

        /// <summary>
        ///     Unsubscribes an action from the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event to unsubscribe from.</param>
        /// <param name="action">Action to remove.</param>
        public void Unsubscribe<T>(T eventType, Action action) where T : Enum
        {
	        var key = (typeof(T), (object)eventType);
            
	        if (_events.ContainsKey(key))
	        {
		        _events[key] -= action;
                
		        // Опционально: удаляем ключ, если подписчиков не осталось
		        if (_events[key] == null || _events[key].Method.Name == "b__0") 
			        _events.Remove(key);
	        }
        }

        /// <summary>
        ///     Triggers the specified event, invoking all subscribed actions if any.
        /// </summary>
        /// <param name="eventType">Event to trigger.</param>
        public void Trigger<T>(T eventType) where T : Enum
        {
	        var key = (typeof(T), (object)eventType);
            
	        if (_events.TryGetValue(key, out var action))
	        {
		        action?.Invoke();
	        }
        }
	}
}