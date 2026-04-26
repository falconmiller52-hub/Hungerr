using FMOD.Studio;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Runtime.Features.Enemy.Thin.States
{
	public class ChaseState : IEnemyState
	{
		private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
		private static readonly int Chase = Animator.StringToHash("Chase");
	
		private readonly ThinEnemyAI _ai;
		private EventInstance _currentSound;
		
		public ChaseState(ThinEnemyAI ai) => _ai = ai;

		public void Enter()
		{
			_ai.Animator.SetFloat(WalkSpeed, _ai.ChaseSpeedMultiplier);
			_ai.Agent.speed = _ai.Animator.GetFloat(WalkSpeed) * _ai.transform.lossyScale.x;
		
			_ai.Animator.SetBool(Chase, true);
			
			_currentSound = _ai.SoundService.PlaySound(_ai.ChaseSounds, _ai.transform.position);
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

		public void Exit()
		{
			_ai.Animator.SetBool(Chase, false);
			_ai.SoundService.StopSound(_currentSound, STOP_MODE.ALLOWFADEOUT);
		}
	}
}