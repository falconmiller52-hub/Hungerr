using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
	public class CounterUI : MonoBehaviour
	{
		[SerializeField] private Image _image;
		[SerializeField] private TMP_Text _textObject;

		private void Start()
		{
			if (_image == null)
				_image.gameObject.SetActive(false);
		}

		public void UpdateUI(float currentValue, float maxValue)
		{
			_textObject.text = $"{currentValue} / {maxValue}";
		}
	}
}
