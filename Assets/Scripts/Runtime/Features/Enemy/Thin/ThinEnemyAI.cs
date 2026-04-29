using Runtime.Common.Services.Audio.Sound;
using Runtime.Common.Services.Pause;
using Runtime.Features.Enemy.Thin.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Debug = UnityEngine.Debug;
using IState = Runtime.Common.Services.StateMachine.IState;
using Random = UnityEngine.Random;
using StateMachine = Runtime.Common.Services.StateMachine.StateMachine;

namespace Runtime.Features.Enemy.Thin
{
	public class ThinEnemyAI : MonoBehaviour, IPausable
	{
		private static readonly int Move = Animator.StringToHash("Move");

		[field: SerializeField] public Animator Animator { get; private set; }
		[field: SerializeField] public NavMeshAgent Agent { get; private set; }

		public ISoundService SoundService => _soundService;
		public StateMachine Machine { get; private set; }
		public EnemySettingData EnemySettingData;

		private ISoundService _soundService;
		private IPauseController _pauseController;
		private IState _lastState;

		private int _currentTargetIndex = -1;
		private Vector2 _smoothDeltaPosition;
		private Vector2 _velocity;

		public Transform Target { get; private set; }
		public Transform[] PatrolPoints { get; private set; }

		[Inject]
		private void Construct(ISoundService soundService, IPauseController pauseController)
		{
			_soundService = soundService;
			_pauseController = pauseController;
		}

		private void Awake()
		{
			_pauseController.Add(this);
			
			Agent.updatePosition = false;
			Agent.updateRotation = true;
		}

		private void Update()
		{
			Machine.CurrentState?.Execute();
			SynchronizeAnimatorAndAgent();
		}

		public void Init(GameObject target, Transform[] patrolPoints)
		{
			Target = target.transform;

			Machine = new StateMachine();
			
			Machine.RegisterState(new PatrolState(this));
			Machine.RegisterState(new ChaseState(this));
			Machine.RegisterState(new LostPlayerState(this));
			Machine.RegisterState(new AttackState(this));
			Machine.RegisterState(new PauseState());

			if (patrolPoints.Length > 0)
				PatrolPoints = patrolPoints;
		}
		
		public void OnAnimationEventInvoked()
		{
			if (Machine.CurrentState is IAnimationEventListener listener)
			{
				listener.OnAnimationEventHandled();
			}
		}

		public void Stop()
		{
			if(Machine.CurrentState is PauseState)
				return;
			
			_lastState = Machine.CurrentState;
			
			Machine.EnterIn<PauseState>();
		}

		public void Resume()
		{
			// TODO : Сделать возврат в последние состояние
			
			// Пока что так. Я не ебу хороший способ возвращаться в то же состояние, где мы были до паузы
			Machine.EnterIn<PatrolState>();
		}

		public bool CanSeePlayer()
		{
			if (Target == null)
			{
				Debug.LogError("No target found");
				return false;
			}

			Vector3 playerDirection = (Target.position - transform.position).normalized;

			// Есть ли препятствие между врагом и игроком
			if (Physics.Raycast(transform.position, playerDirection, out RaycastHit hit,
							    EnemySettingData.DetectionRadius))
			{
				// Препятствия нет
				if (hit.collider.gameObject == Target.gameObject)
				{
					// Игрок близко
					if (Vector3.Distance(transform.position, Target.position) < EnemySettingData.DetectionRadius)
						return true;
					return false;
				}

				// Есть препятствие, не видим игрока
				return false;
			}

			// Игрок далеко
			return false;
		}

		public bool CanAttackPlayer() =>
						Target != null && Vector3.Distance(transform.position, Target.position) <
						EnemySettingData.AttackRadius;

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
	}
}