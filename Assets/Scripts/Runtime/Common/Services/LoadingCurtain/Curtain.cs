using System;
using System.Collections;
using UnityEngine;

namespace Runtime.Common.Services.LoadingCurtain
{
	public class Curtain : MonoBehaviour, ILoadingCurtain
	{
		[SerializeField] private CanvasGroup _curtain;
		[SerializeField] private GameObject _textObject;

		[SerializeField] [Range(0.005f, 0.07f)]
		private float _fadeInSpeed;

		[SerializeField] [Range(0.001f, 0.07f)]
		private float _fadeOutSpeed;

		void Awake()
		{
			transform.SetParent(null);
			DontDestroyOnLoad(this);
		}

		/// <summary>
		/// Метод, который затемняет экран и показывает загрузочный экран
		/// </summary>
		/// <param name="customTime">Время между итерациями затемнения</param>
		/// <param name="needText">Флаг - показывать или нет текст "Loading"</param>
		/// <param name="onEnd">! Коллбек который выполняется сразу после полного затемнения экрана !</param>
		public void Show(float customTime = -1, bool needText = true, Action onEnd = null)
		{
			gameObject.SetActive(true);

			float tempFadeOutSpeed = _fadeOutSpeed;

			if (customTime > -1)
				tempFadeOutSpeed = customTime;

			if (needText)
				_textObject.SetActive(true);
			else
				_textObject.SetActive(false);

			StartCoroutine(DoFadeOut(tempFadeOutSpeed, onEnd));
		}

		/// <summary>
		/// Метод, который рассветляет экран
		/// </summary>
		/// <param name="customTime">Время между итерациями рассветления</param>
		/// <param name="needText">Флаг - показывать или нет текст "Loading"</param>
		/// <param name="onEnd">! Коллбек который выполняется сразу после полного рассветления экрана !</param>
		public void Hide(float customTime = -1, bool needText = true, Action onEnd = null)
		{
			gameObject.SetActive(true);

			float tempFadeInSpeed = _fadeInSpeed;

			if (customTime > -1)
				tempFadeInSpeed = customTime;

			if (needText)
				_textObject.SetActive(true);
			else
				_textObject.SetActive(false);
			
			StartCoroutine(DoFadeIn(tempFadeInSpeed, onEnd));
		}

		IEnumerator DoFadeIn(float fadeInSpeed, Action onEnd)
		{
			while (_curtain.alpha > 0)
			{
				_curtain.alpha -= 0.03f;
				yield return new WaitForSeconds(fadeInSpeed);
			}

			gameObject.SetActive(false);
			onEnd?.Invoke();
		}

		IEnumerator DoFadeOut(float fadeOutSpeed, Action onEnd)
		{
			gameObject.SetActive(true);

			while (_curtain.alpha < 1)
			{
				_curtain.alpha += 0.03f;
				yield return new WaitForSeconds(fadeOutSpeed);
			}
			
			onEnd?.Invoke();
		}
	}
}