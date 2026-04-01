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
            
            var thinAi = FindAnyObjectByType<ThinEnemyAI>();

            if (thinAi != null)
                thinAi.InitPlayer(player);
            else
                Debug.LogError("No Thin AI assigned");
        }
    }
}
