using System;
using System.Collections;
using UnityEngine;

namespace Runtime.Common.Services.LoadingCurtain
{
	public class Curtain : MonoBehaviour, ILoadingCurtain
	{
		[SerializeField] private CanvasGroup _curtain;
		[SerializeField] private GameObject _textObject;

		[SerializeField]
		private float _fadeInDefaultDuraion = 0.5f;

		[SerializeField]
		private float _fadeOutDefaultDuration = 0.5f;

		void Awake()
		{
			transform.SetParent(null);
			DontDestroyOnLoad(this);
		}

		/// <summary>
		/// Метод, который затемняет экран и показывает загрузочный экран
		/// </summary>
		/// <param name="duration">Время между итерациями затемнения</param>
		/// <param name="needText">Флаг - показывать или нет текст "Loading"</param>
		/// <param name="onEnd">! Коллбек который выполняется сразу после полного затемнения экрана !</param>
		public void Show(float duration = -1, bool needText = true, Action onEnd = null)
		{
			gameObject.SetActive(true);

			float tempFadeOutDuration = _fadeOutDefaultDuration;

			if (duration > -1)
				tempFadeOutDuration = duration;

			if (needText)
				_textObject.SetActive(true);
			else
				_textObject.SetActive(false);

			StartCoroutine(DoFadeOut(tempFadeOutDuration, onEnd));
		}

		/// <summary>
		/// Метод, который рассветляет экран
		/// </summary>
		/// <param name="duration">Время между итерациями рассветления</param>
		/// <param name="needText">Флаг - показывать или нет текст "Loading"</param>
		/// <param name="onEnd">! Коллбек который выполняется сразу после полного рассветления экрана !</param>
		public void Hide(float duration = -1, bool needText = true, Action onEnd = null)
		{
			gameObject.SetActive(true);

			float tempFadeInDuration = _fadeInDefaultDuraion;

			if (duration > -1)
				tempFadeInDuration = duration;

			if (needText)
				_textObject.SetActive(true);
			else
				_textObject.SetActive(false);
			
			StartCoroutine(DoFadeIn(tempFadeInDuration, onEnd));
		}

		IEnumerator DoFadeIn(float duration, Action onEnd)
		{
			float elapsed = 0f;
			float startAlpha = _curtain.alpha;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				_curtain.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
				
				yield return null;
			}

			_curtain.alpha = 0f;
			gameObject.SetActive(false);
			onEnd?.Invoke();
		}

		IEnumerator DoFadeOut(float duration, Action onEnd)
		{
			gameObject.SetActive(true);
    
			float elapsed = 0f;
			float startAlpha = _curtain.alpha;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				_curtain.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
				
				yield return null;
			}

			_curtain.alpha = 1f;
			onEnd?.Invoke();
		}
	}
}