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
			_curtain.Show(onEnd:() =>
			{
				_startPanelGroup.gameObject.SetActive(false);
			});
			
			yield return new WaitForSeconds(1); 
			
			_curtain.Hide(0.001f);
		}
	}
}