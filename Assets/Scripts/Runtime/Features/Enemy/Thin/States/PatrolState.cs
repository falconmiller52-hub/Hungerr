using UnityEngine;

namespace Runtime.Features.Enemy.Thin.States
{
    public class PatrolState : IEnemyState
    {
        private ThinEnemyAI _owner;
        private int _nextPoint;
        private float _waitTimer;
        private bool _isWaiting;

        // Константы имен анимаций, чтобы не опечататься
        private const string IdleAnim = "Idle"; 
        private const string WalkAnim = "Walk";

        public PatrolState(ThinEnemyAI owner) => _owner = owner;

        public void Enter()
        {
            _owner.Agent.speed = _owner.PatrolSpeed;
            _isWaiting = false;
            SendToNextPoint();
        }

        public void Execute()
        {
            // 1. Поиск игрока
            if (_owner.CanSeePlayer())
            {
                _owner.ChangeState(new ChaseState(_owner));
                return;
            }

            // 2. Проверка NavMesh
            if (!_owner.Agent.isActiveAndEnabled || !_owner.Agent.isOnNavMesh)
                return;

            // 3. Логика ожидания
            if (_isWaiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0)
                {
                    _isWaiting = false;
                    SendToNextPoint();
                }
                return;
            }

            // 4. Проверка дистанции
            if (!_owner.Agent.pathPending && _owner.Agent.remainingDistance < 0.5f)
            {
                StartWaiting();
            }
        }

        private void StartWaiting()
        {
            _isWaiting = true;
            _waitTimer = _owner.PatrolWaitTime;
            
            _owner.Agent.isStopped = true;
            // Плавный переход в Idle за 0.2 секунды
            _owner.Animator.CrossFade(IdleAnim, 0.2f);
        }

        private void SendToNextPoint()
        {
            if (_owner.PatrolPoints.Length == 0) return;

            _owner.Agent.isStopped = false;
            _owner.Agent.destination = _owner.PatrolPoints[_nextPoint].position;
            
            // Плавный переход в Walk
            _owner.Animator.CrossFade(WalkAnim, 0.2f);
            
            _nextPoint = (_nextPoint + 1) % _owner.PatrolPoints.Length;
        }

        public void Exit()
        {
            if (_owner.Agent.isOnNavMesh)
                _owner.Agent.isStopped = false;
        }
    }
}