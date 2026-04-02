using UnityEngine;

namespace Runtime.Infra.GameRunner
{
    [CreateAssetMenu(fileName = "GameRunnerConfig", menuName = "Configs/GameRunnerConfig")]
    public class GameRunnerConfig : ScriptableObject
    {
        // если Enabled == true то при запуске плей мода с любой сцены пойдет стандарт пайп: бут сцена - меню - геймплей
        // если false то позволяет запустить альтернативный пайп: инициализация глобальных сервисов - запуск сцены с которой апустили плей мод
        public bool Enabled => _enabled;
        
        [SerializeField] private bool _enabled = true;
    }
}