using NaughtyAttributes;
using Runtime.Common.Enums;
using UnityEngine;

namespace Runtime.Features.Enemy
{
	public class EnemyDebugGizmos : MonoBehaviour
	{
		[InfoBox("Красная сфера - радиус атаки\n" +
		         "Зеленая сфера - радиус зрения")]
		
		[Header("So врага который спавнится")]
		[SerializeField] private EnemySettingData _enemySettingData;

		[Header("Enemy type")] 
		[SerializeField] private EnemyType _enemyType;

		private void OnDrawGizmosSelected()
		{
			if (_enemySettingData == null) return;

			// Радиус атаки
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, _enemySettingData.AttackRadius);

			// Радиус обнаружения
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, _enemySettingData.DetectionRadius);
		}
	}
}