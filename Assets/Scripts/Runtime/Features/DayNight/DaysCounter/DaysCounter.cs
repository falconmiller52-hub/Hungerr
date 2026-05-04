using PrimeTween;
using TMPro;
using UnityEngine;

namespace Runtime.Features.DayNight.DaysCounter
{
	public class DaysCounter : MonoBehaviour
	{
		[Header("UI")] [SerializeField] private TMP_Text _dayText;

		[Header("Fade Settings")] [SerializeField]
		private float _fadeDuration = 0.5f;

		[SerializeField] 
		private float _displayDuration = 2f;
		
		private Sequence _fadeSequence;
		
		private void Start()
		{
			if (_dayText != null)
				_dayText.alpha = 0;
		}

		private void OnDestroy()
		{
			_fadeSequence.Stop();
		}

		public void UpdateVisual(int currentDay)
		{
			if (_dayText == null)
				return;

			_fadeSequence.Stop();
			_dayText.text = $"Night: {currentDay}";

			_fadeSequence = Sequence.Create()
				// Появление (Альфа от текущей до 1)
				.Chain(Tween.Alpha(_dayText, startValue: 0, endValue: 1, duration: _fadeDuration))
				// Пауза, пока текст виден
				.ChainDelay(_displayDuration)
				// Исчезновение (Альфа до 0)
				.Chain(Tween.Alpha(_dayText, endValue: 0, duration: _fadeDuration));
		}
	}
}