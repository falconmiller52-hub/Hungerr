using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Features.DayNight.StateMachine
{
	public class PhaseStateMachine : MonoBehaviour
	{
		[SerializeField] private DayCycleVisualChanger _dayCycleVisualChanger;
		
		[Header("Time Settings")]
		[SerializeField, Tooltip("Длительность полных суток в секундах"), Range(1, 3600)]
		private float _nightDuration = 120f;
		
		private Dictionary<Type, GamePhaseState> _states = new Dictionary<Type, GamePhaseState>();
		private GamePhaseState _currentState;

		public DayCycleVisualChanger DayCycleVisualChanger => _dayCycleVisualChanger;
		public float NightDuration => _nightDuration;
		
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
