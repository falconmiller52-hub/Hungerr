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

		public void Show(float customTime = -1, bool needText = true)
		{
			gameObject.SetActive(true);

			float tempFadeOutSpeed = _fadeOutSpeed;

			if (customTime > -1)
				tempFadeOutSpeed = customTime;

			if (needText)
				_textObject.SetActive(true);
			else
				_textObject.SetActive(false);

			StartCoroutine(DoFadeOut(tempFadeOutSpeed));

			// ASK: ↓ Не уверен что данная строка нужна здесь. Т.к. если вызвать метод Show ↓
			// ASK: ↓ корутина не успевает сделать плавный переход и сразу идёт альфа = 1 ↓
			//_curtain.alpha = 1;
		}

		public void Hide(float customTime = -1, bool needText = true)
		{
			gameObject.SetActive(true);

			float tempFadeInSpeed = _fadeInSpeed;

			if (customTime > -1)
				tempFadeInSpeed = customTime;

			if (needText)
				_textObject.SetActive(true);
			else
				_textObject.SetActive(false);
			
			StartCoroutine(DoFadeIn(tempFadeInSpeed));
		}

		IEnumerator DoFadeIn(float fadeInSpeed)
		{
			while (_curtain.alpha > 0)
			{
				_curtain.alpha -= 0.03f;
				yield return new WaitForSeconds(fadeInSpeed);
			}

			gameObject.SetActive(false);
			
			//ASK: Нам бы сюда какие нить колбеки или ивенты  
			//ASK: чтобы было понятно что действие должно происходить только после полного затемнения / засветления
		}

		IEnumerator DoFadeOut(float fadeOutSpeed)
		{
			gameObject.SetActive(true);

			while (_curtain.alpha < 1)
			{
				_curtain.alpha += 0.03f;
				yield return new WaitForSeconds(fadeOutSpeed);
			}
			
			//ASK : Тут так же как и в FadeIn
		}
	}
}