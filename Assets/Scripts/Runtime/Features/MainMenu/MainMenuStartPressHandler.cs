using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Features.MainMenu
{
    /// <summary>
    /// реагирует на нажатие start кнопки в меню и запускает геймплейную сцену
    /// </summary>
    public class MainMenuStartPressHandler : MonoBehaviour
    {
        [SerializeField] private Button _startPressButton;
        [SerializeField] private CanvasGroup _startPanelGroup;
        [SerializeField] private float _startPanelFadeSpeed = 1f;
        
        private void OnEnable()
        {
            _startPressButton.onClick.AddListener(HandleStartPressButton);
        }

        private void OnDisable()
        {
            _startPressButton.onClick.RemoveListener(HandleStartPressButton);
            StopAllCoroutines();
        }

        private void HandleStartPressButton()
        {
            StartCoroutine(ProcessFadeStartPanel());
        }

        private IEnumerator ProcessFadeStartPanel()
        {
            while (_startPanelGroup.alpha > 0)
            {
                _startPanelGroup.alpha -= Time.deltaTime * _startPanelFadeSpeed;
                yield return null;
            }
            
            _startPanelGroup.gameObject.SetActive(false);
        }
    }
}
