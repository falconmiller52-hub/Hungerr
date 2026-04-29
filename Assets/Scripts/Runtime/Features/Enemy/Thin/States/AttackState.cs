using Runtime.Features.Health;
using UnityEngine;

namespace Runtime.Features.Enemy.Thin.States
{
	public class AttackState : IEnemyState, IAnimationEventListener
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

				if (_cooldownTimer >= _ai.AttackCooldown)
				{
					// Время отдыха вышло — возвращаемся к патрулю
					_ai.ChangeState<PatrolState>();
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
			float maxDistance = _ai.AttackRadius; // Насколько далеко летит "коробка"
			
			if (Physics.BoxCast(origin, halfExtents, direction, out RaycastHit hit, orientation, maxDistance))
			{
				var damageable = hit.collider.GetComponentInParent<IDamageable>();
        
				if (damageable != null)
				{
					damageable.ApplyDamage(_ai.AttackDamage);
					Debug.Log($"<color=yellow>BoxCast попал по: {hit.collider.name}</color>");
				}
			}
		}
	}
}