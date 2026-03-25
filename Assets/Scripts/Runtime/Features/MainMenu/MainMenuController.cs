using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Runtime.Features.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _exitGameButton;
        [SerializeField] private string _gameplaySceneName;

        private void OnEnable()
        {
            _startGameButton.onClick.AddListener(HandleStartGame);
            _exitGameButton.onClick.AddListener(HandleExitGame);
        }

        private void OnDisable()
        {
            _startGameButton.onClick.RemoveListener(HandleStartGame);
            _exitGameButton.onClick.RemoveListener(HandleExitGame);
        }

        private void HandleExitGame()
        {
            Application.Quit();
        }

        private void HandleStartGame()
        {
            SceneManager.LoadScene(_gameplaySceneName);
        }
    }
}
