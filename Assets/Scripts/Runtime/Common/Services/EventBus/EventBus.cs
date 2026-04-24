using System;
using System.Collections.Generic;

namespace Runtime.Common.Services.EventBus
{
	public class EventBus : IDisposable
	{
		// Используем Delegate вместо Action, чтобы хранить и Action, и Action<TData>
		private readonly Dictionary<(Type, object), Delegate> _events = new();

		public void Dispose()
		{
			_events.Clear();
		}

		// --- ОБЫЧНЫЕ ИВЕНТЫ (без данных) ---
		public void Subscribe<T>(T eventType, Action action) where T : Enum
		{
			var key = (typeof(T), (object)eventType);
			_events[key] = Delegate.Combine(_events.GetValueOrDefault(key), action);
		}

		public void Unsubscribe<T>(T eventType, Action action) where T : Enum
		{
			var key = (typeof(T), (object)eventType);
			if (_events.TryGetValue(key, out var del))
			{
				var newDel = Delegate.Remove(del, action);
				if (newDel == null) _events.Remove(key);
				else _events[key] = newDel;
			}
		}

		public void Trigger<T>(T eventType) where T : Enum
		{
			var key = (typeof(T), (object)eventType);
			if (_events.TryGetValue(key, out var del) && del is Action action)
			{
				action.Invoke();
			}
		}

		// --- ИВЕНТЫ С ДАННЫМИ (Action<TData>) ---
		public void Subscribe<T, TData>(T eventType, Action<TData> action) where T : Enum
		{
			var key = (typeof(T), (object)eventType);
			_events[key] = Delegate.Combine(_events.GetValueOrDefault(key), action);
		}

		public void Unsubscribe<T, TData>(T eventType, Action<TData> action) where T : Enum
		{
			var key = (typeof(T), (object)eventType);
			if (_events.TryGetValue(key, out var del))
			{
				var newDel = Delegate.Remove(del, action);
				if (newDel == null) _events.Remove(key);
				else _events[key] = newDel;
			}
		}

		public void Trigger<T, TData>(T eventType, TData data) where T : Enum
		{
			var key = (typeof(T), (object)eventType);
			if (_events.TryGetValue(key, out var del) && del is Action<TData> action)
			{
				action.Invoke(data);
			}
		}
	}
}