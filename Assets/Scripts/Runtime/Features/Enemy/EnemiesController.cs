using System;
using Runtime.Features.Enemy.Thin;
using Unity.AI.Navigation;
using UnityEngine;

namespace Runtime.Features.Enemy
{
    public class EnemiesController : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface _navMeshSurface;
        
        public void Init(GameObject player)
        {
            if (_navMeshSurface == null)
                _navMeshSurface = FindAnyObjectByType<NavMeshSurface>();
            
            if (_navMeshSurface.navMeshData == null)
                _navMeshSurface.BuildNavMesh();

            if (player == null)
            {
                Debug.LogError("EnemiesController::Init() Player is null");
                return;
            }
            
            var enemyAis = FindObjectsOfType<ThinEnemyAI>();

            if (enemyAis == null)
            {
                Debug.LogError("EnemiesController::Init() No AI assigned");
                return;
            }
            
            foreach (var ai in enemyAis)
            {
                ai.InitPlayer(player);
            }
        }
    }
}
