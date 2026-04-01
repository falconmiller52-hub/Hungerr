using Runtime.Features.Enemy.Thin.States;
using UnityEngine;

public class LostPlayerState : IEnemyState
{
	private ThinEnemyAI _ai;
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
			_ai.ChangeState(new ChaseState(_ai));
			return;
		}

		_timer += Time.deltaTime;
		
		if (_timer >= 3f)
		{
			_ai.ChangeState(new PatrolState(_ai));
		}
	}
	public void Exit() {}
}