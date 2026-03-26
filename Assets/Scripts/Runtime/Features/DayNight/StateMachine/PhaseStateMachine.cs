using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Features.DayNight.StateMachine
{
	public class PhaseStateMachine : MonoBehaviour
	{
		[field: SerializeField] public DayCycleVisualChanger DayCycleVisualChanger { get; private set; }
		
		[field: SerializeField, Tooltip("Длительность полных суток в секундах"), Range(1, 3600)]
		public float NightDuration { get; private set; } = 120f;
		
		[field: SerializeField] public Transform DayStartLocationtransform { get; private set; } 
		[field: SerializeField] public Transform NightStartLocationtransform { get; private set; } 
		
		private Dictionary<Type, GamePhaseState> _states = new Dictionary<Type, GamePhaseState>();
		private GamePhaseState _currentState;
		
		public void EnterIn<TState>() where TState : GamePhaseState
		{
			if (_states.TryGetValue(typeof(TState), out var state))
			{
				_currentState?.Exit();
				_currentState = state;
				_currentState.Enter();
			}
		}

		public void RegisterState<TState>(TState state) where TState : GamePhaseState
		{
			if (_states.ContainsKey(typeof(TState)))
				throw new ArgumentException("State already existing in States Map: " + typeof(TState));

			_states.Add(typeof(TState), state);
		}

		private void Update()
		{
			_currentState?.Update();
		}
	}
}
