using Runtime.Common.Extensions;
using Runtime.Features.Sounds;
using UnityEngine;

namespace Runtime.Features.Enemy.Thin.States
{
	public class ChaseState : IEnemyState
	{
		private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
		private static readonly int Chase = Animator.StringToHash("Chase");
	
		private readonly ThinEnemyAI _ai;
		private SoundData _currentSound;
		private bool _isActive = false;
		public ChaseState(ThinEnemyAI ai) => _ai = ai;

		public void Enter()
		{
			_isActive = true;
			
			_ai.Animator.SetFloat(WalkSpeed, _ai.ChaseSpeedMultiplier);
			_ai.Agent.speed = _ai.Animator.GetFloat(WalkSpeed) * _ai.transform.lossyScale.x;
		
			_ai.Animator.SetBool(Chase, true);
			
			_currentSound = _ai.ChaseSounds.Random();
			_ai.AudioService.PlaySfx(_currentSound, _ai.transform.position, onEnd: SetNewSound);
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
			_isActive = false;
			_ai.Animator.SetBool(Chase, false);
			_ai.AudioService.StopPlaying(_currentSound);
		}

		private void SetNewSound(SoundData currentPlayedData)
		{
			if (!_isActive) 
				return;
			
			_currentSound = _ai.ChaseSounds.RandomExcept(currentPlayedData);
			_ai.AudioService.PlaySfx(_currentSound, _ai.transform.position, onEnd: SetNewSound);
		}
	}
}