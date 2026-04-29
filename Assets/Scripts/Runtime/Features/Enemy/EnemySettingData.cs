using System;
using FMODUnity;
using UnityEngine;

namespace Runtime.Features.Enemy
{
	[Serializable]
	[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Enemy settings")]
	public class EnemySettingData : ScriptableObject
	{
		[field: Header("Patrol Settings")]
		[field: SerializeField] public EventReference PatrolSounds { get; private set; }
		[field: SerializeField] public float PatrolSpeedMultiplier { get; private set; }

		
		[field: Header("Chase Settings")]
		[field: SerializeField] public float ChaseSpeedMultiplier { get; private set; }
		[field: SerializeField] public float DetectionRadius { get; private set; } = 10f;
		[field: SerializeField] public EventReference ChaseSounds { get; private set; }

		
		[field: Header("Attack Settings")] 
		[field: SerializeField] public float AttackRadius { get; private set; } = 2f;
		[field: SerializeField] public int AttackDamage { get; private set; } = 10;
		[field: SerializeField] public float AttackCooldown { get; private set; } = 1.5f;
	}
}