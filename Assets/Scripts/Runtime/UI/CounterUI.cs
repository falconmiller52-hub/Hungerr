using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
	public class CounterUI : MonoBehaviour
	{
		[SerializeField] private Image _image;
		[SerializeField] private TMP_Text _currentValueTextObject;
		[SerializeField] private TMP_Text _maxValueTextObject;

		private void Start()
		{
			if (_image == null)
				_image.gameObject.SetActive(false);
		}

		public void UpdateUI(float currentValue, float maxValue)
		{
			_currentValueTextObject.text = currentValue.ToString();
			_maxValueTextObject.text = $"/{maxValue}";
		}
	}
}
