using Runtime.Features.Enemy.Thin;
using Unity.AI.Navigation;
using UnityEngine;

namespace Runtime.Features.Enemy
{
	/// <summary>
	/// скрипт который инитит всех врагов на сцене, задает их таргета - игрока
	/// </summary>
	public class EnemiesBootstrap : MonoBehaviour
	{
		[SerializeField] private NavMeshSurface _navMeshSurface;

		public void Init(GameObject targetPlayer)
		{
			if (_navMeshSurface == null)
				_navMeshSurface = FindAnyObjectByType<NavMeshSurface>();

			if (_navMeshSurface.navMeshData == null)
				_navMeshSurface.BuildNavMesh();

			if (targetPlayer == null)
			{
				Debug.LogError("EnemiesController::Init() Player is null");
				return;
			}

			ThinEnemyAI[] enemyAis = FindObjectsByType<ThinEnemyAI>(FindObjectsSortMode.None);

			if (enemyAis == null)
			{
				Debug.LogError("EnemiesController::Init() No AI assigned");
				return;
			}

			foreach (var ai in enemyAis)
			{
				ai.InitPlayer(targetPlayer);
			}
		}
	}
}