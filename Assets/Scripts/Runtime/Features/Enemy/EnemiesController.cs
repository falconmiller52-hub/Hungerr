using Runtime.Features.Enemy.Thin;
using UnityEngine;

namespace Runtime.Features.Enemy
{
    public class EnemiesController : MonoBehaviour
    {
        [SerializeField] private ThinEnemyAI _thinAI;
    
        public void Init(GameObject player)
        {
            if (_thinAI != null)
                _thinAI.InitPlayer(player);
        }
    }
}
