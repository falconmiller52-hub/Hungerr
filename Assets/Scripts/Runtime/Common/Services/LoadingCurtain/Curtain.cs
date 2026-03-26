using System.Collections;
using UnityEngine;

namespace Runtime.Common.Services.LoadingCurtain
{
	public class Curtain : MonoBehaviour, ILoadingCurtain
	{
		[SerializeField] CanvasGroup _curtain;

		[SerializeField] [Range(0.005f, 0.07f)]
		float _fadeInSpeed;

		void Awake()
		{
			transform.SetParent(null);
			DontDestroyOnLoad(this);
		}

		public void Show()
		{
			gameObject.SetActive(true);
			_curtain.alpha = 1;
		}

		public void Hide()
		{
			gameObject.SetActive(true);
			StartCoroutine(DoFadeIn());
		}

		IEnumerator DoFadeIn()
		{
			while (_curtain.alpha > 0)
			{
				_curtain.alpha -= 0.03f;
				yield return new WaitForSeconds(_fadeInSpeed);
			}

			gameObject.SetActive(false);
		}
	}
}