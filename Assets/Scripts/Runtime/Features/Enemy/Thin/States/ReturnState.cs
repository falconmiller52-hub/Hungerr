namespace Runtime.Features.Enemy.Thin.States
{
	public class ReturnState : IEnemyState
	{
		private ThinEnemyAI _owner;

		public ReturnState(ThinEnemyAI owner) => this._owner = owner;

		public void Enter() => _owner.Agent.destination = _owner.StartPoint.position;

		public void Execute()
		{
			if (_owner.CanSeePlayer()) 
			{
				_owner.ChangeState(new ChaseState(_owner));
				return;
			}

			if (!_owner.Agent.pathPending && _owner.Agent.remainingDistance < 0.2f) 
			{
				_owner.ChangeState(new PatrolState(_owner));
			}
		}

		public void Exit() { }
	}
}