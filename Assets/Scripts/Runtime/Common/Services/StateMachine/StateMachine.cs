using System;
using System.Collections.Generic;

namespace Runtime.Common.Services.StateMachine
{
	public class StateMachine : IStateMachine
	{
		public event Action<IState> StateChanged;
		
		readonly Dictionary<Type, IState> _states;

		public IState CurrentState { get; private set; }

		public StateMachine()
		{
			_states = new Dictionary<Type, IState>();
		}

		public void EnterIn<TState>() where TState : IState
		{
			if (_states.TryGetValue(typeof(TState), out var state))
			{
				CurrentState?.Exit();
				CurrentState = state;
				CurrentState.Enter();

				StateChanged?.Invoke(CurrentState);
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