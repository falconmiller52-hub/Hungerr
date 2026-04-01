using UnityEngine;

namespace Runtime.Features.Enemy.Thin.States
{
	public class PatrolState : IEnemyState
	{
		private ThinEnemyAI _ai;
		public PatrolState(ThinEnemyAI ai) => _ai = ai;

		public void Enter()
		{
			Debug.Log("Set Patrol State");
			_ai.Animator.SetFloat("WalkSpeed", _ai.PatrolSpeedMultiplier);
			_ai.Agent.speed = _ai.Animator.GetFloat("WalkSpeed");
		}

		public void Execute()
		{
			if (_ai.CanSeePlayer())
			{
				_ai.ChangeState(new ChaseState(_ai));
				return;
			}

			if (!_ai.Agent.pathPending && _ai.Agent.remainingDistance <= _ai.Agent.stoppingDistance)
			{
				_ai.SetNewAgentPoint();
			}
		}
		public void Exit() {}
	}
}