using System;
using System.Collections.Generic;
using FMODUnity;
using Runtime.Common.Services.Audio;
using Runtime.Common.Services.Audio.Sound;
using Runtime.Features.DayNight.StateMachine;
using Runtime.Features.Enemy.Thin.States;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

namespace Runtime.Features.Enemy.Thin
{
	public class ThinEnemyAI : MonoBehaviour
	{
		private static readonly int Move = Animator.StringToHash("Move");
		
		[field: SerializeField] public Animator Animator { get; private set; }
		[field: SerializeField] public NavMeshAgent Agent { get; private set; }
		
		public EnemySettingData EnemySettingData;

		private Dictionary<Type, IEnemyState> _states = new();
		private IEnemyState _currentState;
		private int _currentTargetIndex = -1;
		private Vector2 _smoothDeltaPosition;
		private Vector2 _velocity;
		private ISoundService _soundService;

		public Transform Target { get; private set; }
		public Transform[] PatrolPoints { get; private set; }
		public ISoundService SoundService => _soundService;
		
		[Inject]
		private void Construct(ISoundService soundService)
		{
			_soundService = soundService;
		}
		
		private void Awake()
		{
			Agent.updatePosition = false;
			Agent.updateRotation = true;
		}
		
		private void OnDisable()
		{
			_states.Clear();
		}

		private void Update()
		{
			_currentState?.Execute();
			SynchronizeAnimatorAndAgent();
		}
		
		public void Init(GameObject target, Transform[] patrolPoints)
		{
			Target = target.transform;
			
			RegisterState(new PatrolState(this));
			RegisterState(new ChaseState(this));
			RegisterState(new LostPlayerState(this));
			RegisterState(new AttackState(this));
			
			if (patrolPoints.Length > 0)
				PatrolPoints = patrolPoints;
		}
		
		public void RegisterState<TState>(TState state) where TState : IEnemyState
		{
			if (_states.ContainsKey(typeof(TState)))
				throw new ArgumentException("State already existing in States Map: " + typeof(TState));

			_states.Add(typeof(TState), state);
		}

		public void ChangeState<TState>() where TState : IEnemyState
		{
			if (_states.TryGetValue(typeof(TState), out var state))
			{
				if(state == _currentState)
					return;
				
				_currentState?.Exit();
				_currentState = state;
				_currentState.Enter();
			}
		}

		public bool CanSeePlayer() =>
			Target != null && Vector3.Distance(transform.position, Target.position) < EnemySettingData.DetectionRadius;

		public bool CanAttackPlayer() =>
			Target != null && Vector3.Distance(transform.position, Target.position) < EnemySettingData.AttackRadius;

		public void SetNewAgentPoint()
		{
			if (PatrolPoints == null || PatrolPoints.Length == 0)
				return;

			int nextIndex = Random.Range(0, PatrolPoints.Length);
			if (nextIndex == _currentTargetIndex) nextIndex = (nextIndex + 1) % PatrolPoints.Length;
			_currentTargetIndex = nextIndex;

			Agent.SetDestination(PatrolPoints[_currentTargetIndex].position);
		}

		private void SynchronizeAnimatorAndAgent()
		{
			Vector3 worldDeltaPosition = Agent.nextPosition - transform.position;
			worldDeltaPosition.y = 0;

			float dx = Vector3.Dot(transform.right, worldDeltaPosition);
			float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
			Vector2 deltaPosition = new Vector2(dx, dy);

			float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
			_smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, deltaPosition, smooth);

			_velocity = _smoothDeltaPosition / Time.deltaTime;

			// логика замедления возле цели
			if (Agent.remainingDistance <= Agent.stoppingDistance)
			{
				_velocity = Vector2.Lerp(Vector2.zero, _velocity, Agent.remainingDistance / Agent.stoppingDistance);
			}

			bool shouldMove = _velocity.magnitude > 0.5f && Agent.remainingDistance > Agent.stoppingDistance;
			Animator.SetBool(Move, shouldMove);

			// подтягивание трансформа к агенту
			if (worldDeltaPosition.magnitude > Agent.radius / 2f)
			{
				transform.position = Vector3.Lerp(transform.position, Agent.nextPosition, smooth);
			}
		}

		public void OnAnimationEventInvoked()
		{
			if (_currentState is IAnimationEventListener listener)
			{
				listener.OnAnimationEventHandled();
			}
		}
	}
}