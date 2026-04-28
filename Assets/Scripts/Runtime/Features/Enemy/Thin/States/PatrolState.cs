using FMOD.Studio;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Runtime.Features.Enemy.Thin.States
{
	public class PatrolState : IEnemyState
	{
		private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
		
		private readonly ThinEnemyAI _ai;
		private EventInstance _currentSound;

		public PatrolState(ThinEnemyAI ai) => _ai = ai;

		public void Enter()
		{
			_ai.Animator.SetFloat(WalkSpeed, _ai.PatrolSpeedMultiplier);
			_ai.Agent.speed = _ai.Animator.GetFloat(WalkSpeed) * _ai.transform.lossyScale.x;
			
			_currentSound = _ai.SoundService.PlaySound(_ai.PatrolSounds, _ai.transform.position);
		}

		public void Execute()
		{
			if (_ai.CanSeePlayer())
			{
				_ai.ChangeState<ChaseState>();
				return;
			}

			if (!_ai.Agent.pathPending && _ai.Agent.remainingDistance <= _ai.Agent.stoppingDistance)
				_ai.SetNewAgentPoint();
		}

		public void Exit()
		{
			_ai.SoundService.StopSound(_currentSound, STOP_MODE.ALLOWFADEOUT);
		}
	}
}