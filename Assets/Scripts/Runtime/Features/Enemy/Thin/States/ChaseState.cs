using UnityEngine;

namespace Runtime.Features.Enemy.Thin.States
{
	public class ChaseState : IEnemyState
	{
		private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
		private static readonly int Chase = Animator.StringToHash("Chase");
	
		private readonly ThinEnemyAI _ai;
		public ChaseState(ThinEnemyAI ai) => _ai = ai;

		public void Enter()
		{
			_ai.Animator.SetFloat(WalkSpeed, _ai.ChaseSpeedMultiplier);
			_ai.Agent.speed = _ai.Animator.GetFloat(WalkSpeed);
		
			_ai.Animator.SetBool(Chase, true);
		}

		public void Execute()
		{
			if (!_ai.CanSeePlayer())
			{
				_ai.ChangeState(new LostPlayerState(_ai));
				return;
			}

			if (_ai.CanAttackPlayer())
			{
				_ai.ChangeState(new AttackState(_ai));
				return;
			}
		
			_ai.Agent.SetDestination(_ai.Target.position);
		}

		public void Exit() => _ai.Animator.SetBool(Chase, false);
		public void OnAnimationEventHandled()
		{
		
		}
	}
}