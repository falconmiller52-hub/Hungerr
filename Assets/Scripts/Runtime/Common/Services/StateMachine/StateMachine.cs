using System;
using System.Collections.Generic;

namespace Runtime.Common.Services.StateMachine
{
	public class StateMachine : IStateMachine
	{
		public event Action<IState> StateChanged;
		
		readonly Dictionary<Type, IState> _states;
		
		IState _currentState;

		public StateMachine()
		{
			_states = new Dictionary<Type, IState>();
		}

		public void EnterIn<TState>() where TState : IState
		{
			if (_states.TryGetValue(typeof(TState), out var state))
			{
				_currentState?.Exit();
				_currentState = state;
				_currentState.Enter();

				StateChanged?.Invoke(_currentState);
			}
		}

		public void RegisterState<TState>(TState state) where TState : IState
		{
			if (_states.ContainsKey(typeof(TState)))
				throw new ArgumentException("State already existing in States Map: " + typeof(TState));

			_states.Add(typeof(TState), state);
		}
		
	}
}