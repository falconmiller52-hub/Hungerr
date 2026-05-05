using Runtime.Common.Services.StateMachine;
using Runtime.Features.Health;
using UnityEngine;

namespace Runtime.Features.Enemy.Thin.States
{
	public class AttackState : IState, IAnimationEventListener
	{
		private static readonly int Attack = Animator.StringToHash("Attack");
		
		private readonly ThinEnemyAI _ai;
		private bool _isAttackFinished;
		private float _cooldownTimer;
		
		public AttackState(ThinEnemyAI ai) => _ai = ai;
		
		public void Enter()
		{
			// стартим анимацию
			_ai.Agent.ResetPath();
			_ai.Agent.velocity = Vector3.zero;
			_ai.Animator.SetTrigger(Attack);
		}

		public void Execute()
		{
			if (_isAttackFinished)
			{
				_cooldownTimer += Time.deltaTime;

				if (_cooldownTimer >= _ai.EnemySettingData.AttackCooldown)
				{
					// Время отдыха вышло — возвращаемся к патрулю
					_ai.Machine.EnterIn<PatrolState>();
				}
			}
		}

		public void Exit()
		{
			_cooldownTimer = 0;
			_isAttackFinished = false;
		}

		public void OnAnimationEventHandled()
		{
			DoDamageCheck();
			
			_isAttackFinished = true;
		}
		
		private void DoDamageCheck()
		{
			Vector3 size = new Vector3(1.5f, 1.0f, 0.5f); // Ширина, Высота, Толщина
			Vector3 halfExtents = size / 2f;
			
			Vector3 origin = _ai.transform.position + Vector3.up * 1.0f; 
			Vector3 direction = _ai.transform.forward;
			Quaternion orientation = _ai.transform.rotation;
			float maxDistance = _ai.EnemySettingData.AttackRadius; // Насколько далеко летит "коробка"

            if (Physics.BoxCast(origin, halfExtents, direction, out RaycastHit hit, orientation, maxDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                    damageable?.ApplyDamage(_ai.EnemySettingData.AttackDamage);
                }
            }
        }
	}
}