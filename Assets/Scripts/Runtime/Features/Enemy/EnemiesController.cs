using Runtime.Features.Enemy.Thin;
using UnityEngine;

namespace Runtime.Features.Enemy
{
    public class EnemiesController : MonoBehaviour
    {
        public void Init(GameObject player)
        {
            var thinAi = FindAnyObjectByType<ThinEnemyAI>();

            if (thinAi != null)
            {
                thinAi.InitPlayer(player);
            }
            else
            {
                Debug.LogError("No Thin AI assigned");
            }
        }
    }
}
