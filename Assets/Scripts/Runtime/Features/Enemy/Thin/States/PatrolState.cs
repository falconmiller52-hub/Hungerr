using FMODUnity;
using Runtime.Common.Extensions;
using UnityEngine;

namespace Runtime.Features.Enemy.Thin.States
{
	public class PatrolState : IEnemyState
	{
		private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
		
		private readonly ThinEnemyAI _ai;
		private EventReference _currentSound;
		private bool _isActive = false;

		public PatrolState(ThinEnemyAI ai) => _ai = ai;

		public void Enter()
		{
			_isActive = true;
			
			_ai.Animator.SetFloat(WalkSpeed, _ai.PatrolSpeedMultiplier);
			_ai.Agent.speed = _ai.Animator.GetFloat(WalkSpeed) * _ai.transform.lossyScale.x;
			
			_currentSound = _ai.PatrolSounds.Random();
			_ai.AudioService.PlaySfx(_currentSound, _ai.transform.position, onEnd: SetNewSound);
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

		public void Exit()
		{
			_isActive = false;
			_ai.AudioService.StopPlaying(_currentSound);
		}
		
		private void SetNewSound(EventReference currentPlayedData)
		{
			if (_ai == null || !_isActive) 
				return;
			
			_currentSound = _ai.PatrolSounds.RandomExcept(currentPlayedData);
			_ai.AudioService.PlaySfx(_currentSound, _ai.transform.position, onEnd: SetNewSound);
		}
	}
}