using Runtime.Features.Enemy.Thin.States;
using Runtime.Features.Player.Movement;
using UnityEngine;
using UnityEngine.AI;
using IEnemyState = Runtime.Features.Enemy.Thin.States.IEnemyState;

namespace Runtime.Features.Enemy.Thin
{
    public class ThinEnemyAI : MonoBehaviour
    {
        [field: Header("Settings")]
        [field: SerializeField] public Transform[] PatrolPoints { get; private set; }
        [field: SerializeField] public Transform StartPoint { get; private set; }
        [field: SerializeField] public float PatrolSpeed { get; private set; } = 2f;
        [field: SerializeField] public float ChaseSpeed { get; private set; } = 5f;
        [field: SerializeField] public float DetectionRadius { get; private set; } = 10f;
    
        [field: Header("Параметры патруля")]
        [field: SerializeField] public float PatrolWaitTime { get; private set; } = 3f; // Время ожидания на точках
        [field: SerializeField] public Animator Animator { get; private set; }
        
        public NavMeshAgent Agent { get; private set; }
        public Transform Target  { get; private set; }
        private IEnemyState _currentState;

        void Start()
        {
            Agent = GetComponent<NavMeshAgent>();

            Target = FindObjectOfType<PlayerMovement>().transform;

            if (Target == null)
            {
                Debug.LogError("ThinEnemyAI::Start() No target found");
            }
            
            // Устанавливаем начальное состояние
            ChangeState(new PatrolState(this));
        }

        void Update()
        {
            // Просто запускаем Execute текущего класса
            _currentState?.Execute();
        }

        public void ChangeState(IEnemyState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public bool CanSeePlayer()
        {
            return Vector3.Distance(transform.position, Target.position) < DetectionRadius;
        }

        public void PlayChaseSound(bool play)
        {
            // var audio = GetComponent<AudioSource>();
            //
            // if (play) 
            //     audio.Play(); else audio.Stop();
        }
    }
}
