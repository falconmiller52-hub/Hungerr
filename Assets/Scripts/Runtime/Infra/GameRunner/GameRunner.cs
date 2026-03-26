using Runtime.Common.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Infra.GameRunner
{
    public class GameRunner
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void InitBootstrapScene()
        {
            bool enabled = IsEnabled();
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (enabled)
            {
                // Принудительный запуск с Boot (Build Index 0)
                if (activeSceneIndex != 0)
                {
                    SceneManager.LoadScene(0);
                }
            }
            else
            {
                // Быстрый запуск: остаемся на текущей сцене, 
                // но говорим StateMachine пропустить меню.
                QuickStartBridge.IsQuickStart = true;
                QuickStartBridge.SceneName = SceneManager.GetActiveScene().name;
            }
        }

        private static bool IsEnabled()
        {
            var config = Resources.Load<GameRunnerConfig>("GameRunnerConfig");

            if (config == null)
            {
                Debug.LogWarning("GameRunnerConfig не найден в папке Resources! Проверьте путь.");
                return true;
            }
            
            return config.Enabled;
        }
    }
}