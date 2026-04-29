using System;
using NaughtyAttributes;
using Runtime.Common.Enums;
using Unity.VisualScripting;
using UnityEngine;

namespace Runtime.Features.Enemy
{
	public class EnemyDebugGizmos : MonoBehaviour
	{
		[InfoBox("Красная сфера - радиус атаки\n" +
		         "Зеленая сфера - радиус зрения")]
		
		[Header("So всех врагов")] 
		[SerializeField] private EnemySettingData _thinData;

		[Header("Enemy type")] 
		[SerializeField] private EnemyType _enemyType;
		
		private void OnDrawGizmosSelected()
		{
			switch (_enemyType)
			{
				case EnemyType.Thin:
				{
					// Радиус атаки
					Gizmos.color = Color.red;
					Gizmos.DrawWireSphere(transform.position, _thinData.AttackRadius);
					
					// Радиус обнаружения
					Gizmos.color = Color.green;
					Gizmos.DrawWireSphere(transform.position, _thinData.DetectionRadius);
					break;
				}
			}
			

		}
	}
}