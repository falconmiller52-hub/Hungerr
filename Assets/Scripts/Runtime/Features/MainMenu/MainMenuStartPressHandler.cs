using System.Collections;
using Runtime.Common.Services.LoadingCurtain;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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

		private ILoadingCurtain _curtain;

		[Inject]
		public void Construct(ILoadingCurtain curtain)
		{
			_curtain = curtain;
		}
		
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
			_curtain.Show(0.001f);

			yield return new WaitForSeconds(1); // Заглушка ожидания затемнения

			_startPanelGroup.gameObject.SetActive(false);

			yield return new WaitForSeconds(0.2f); // Заглушка чтобы объект точно успел выключится

			_curtain.Hide(0.001f);
		}
	}
}