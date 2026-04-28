using UnityEngine;

namespace Runtime.Features.Enemy.Thin.States
{
	public class LostPlayerState : IEnemyState
	{
		private readonly ThinEnemyAI _ai;
		private float _timer;

		public LostPlayerState(ThinEnemyAI ai) => _ai = ai;

		public void Enter()
		{
			_timer = 0;
			_ai.Agent.ResetPath(); 
		}

		public void Execute()
		{
			if (_ai.CanSeePlayer())
			{
				_ai.ChangeState<ChaseState>();
				return;
			}

			_timer += Time.deltaTime;
		
			if (_timer >= 3f)
			{
				_ai.ChangeState<PatrolState>();
			}
		}
		
		public void Exit() {}
	}
}