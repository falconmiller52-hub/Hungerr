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
					_ai.ChangeState(new PatrolState(_ai));
				}
			}
		}

		public void Exit()
		{
			
		}

		public void OnAnimationEventHandled()
		{
			DoDamageCheck();
			
			_isAttackFinished = true;
		}
		
		private void DoDamageCheck()
		{
			// Позиция начала каста (чуть впереди врага на уровне груди/рук)
			Vector3 origin = _ai.transform.position + Vector3.up * 1f;
			Vector3 direction = _ai.transform.forward;

			// SphereCast летит вперед на AttackRange
			if (Physics.SphereCast(origin, _ai.AttackRadius, direction, out RaycastHit hit, _ai.AttackRadius))
			{
				// Если у игрока есть компонент здоровья (например, Health)
				// if (hit.collider.TryGetComponent(out Health health)) health.TakeDamage(_ai.AttackDamage);
                
				Debug.Log($"Пизданул игрока! Объект: {hit.collider.name}");
			}
			else
			{
				Debug.Log("Промахнулся!");
			}
		}
	}
}