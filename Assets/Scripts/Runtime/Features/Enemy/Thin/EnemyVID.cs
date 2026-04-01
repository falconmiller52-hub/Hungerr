using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyVID : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform[] _testTargets;
    
    private Vector2 _velocity;
    private Vector2 _smoothDeltaPosition;
    private int _currentTargetIndex = -1;
    
    private void Awake()
    {
        _agent.updatePosition = false;
        _agent.updateRotation = true;
        
        SetNewAgentPoint();
    }

    private void Update()
    {
        SynchronizeAnimatorAndAgent();
        
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            SetNewAgentPoint();
        }
    }

    private void SynchronizeAnimatorAndAgent()
    {
        Vector3 worldDeltaPosition = _agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        _smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, deltaPosition, smooth);

        _velocity = _smoothDeltaPosition / Time.deltaTime;
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _velocity = Vector2.Lerp(
                Vector2.zero, 
                _velocity, 
                _agent.remainingDistance / _agent.stoppingDistance
            );
        }

        bool shouldMove = _velocity.magnitude > 0.5f
            && _agent.remainingDistance > _agent.stoppingDistance;

        _animator.SetBool("Move", shouldMove);

        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > _agent.radius / 2f)
        {
            transform.position = Vector3.Lerp(
                _animator.rootPosition,
                _agent.nextPosition,
                smooth
            );
        }
    }
    
    private void SetNewAgentPoint()
    {
        if (_testTargets == null || _testTargets.Length == 0) return;

        int nextIndex = Random.Range(0, _testTargets.Length);
        if (nextIndex == _currentTargetIndex) nextIndex = (nextIndex + 1) % _testTargets.Length;

        _currentTargetIndex = nextIndex;
        _agent.SetDestination(_testTargets[_currentTargetIndex].position);
    }
}