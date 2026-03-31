using UnityEngine;

namespace Runtime.Features.Enemy.Thin.States
{
	public class ChaseState : IEnemyState
	{
		private ThinEnemyAI _owner;
		private float _loseTargetTimer;

		public ChaseState(ThinEnemyAI owner) => _owner = owner;

		public void Enter()
		{
			_owner.Agent.speed = _owner.ChaseSpeed;
			_owner.PlayChaseSound(true);
		}

		public void Execute()
		{
			if (_owner.CanSeePlayer())
			{
				_loseTargetTimer = 5f; // Сброс таймера
				_owner.Agent.destination = _owner.Target.position;
			}
			else
			{
				_loseTargetTimer -= Time.deltaTime;
				if (_loseTargetTimer <= 0) {
					_owner.ChangeState(new ReturnState(_owner));
				}
			}
		}

		public void Exit() => _owner.PlayChaseSound(false);
	}
}