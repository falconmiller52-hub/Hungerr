using UnityEngine;

namespace Runtime.Infra.GameRunner
{
    [CreateAssetMenu(fileName = "GameRunnerConfig", menuName = "Configs/GameRunnerConfig")]
    public class GameRunnerConfig : ScriptableObject
    {
        public bool Enabled => _enabled;
        
        [SerializeField] private bool _enabled = true;
    }
}