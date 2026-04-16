using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.UI
{
	// класс для добавления фичи мерцающего квадрата возле выбранной опции в меню
	public class MenuButtonHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private GameObject _buttonSelectionTag;
		[SerializeField] private RectTransform _buttonTransform;

		private void OnEnable()
			=> HideHelper();

		private void OnDisable()
			=> HideHelper();

		public void OnPointerEnter(PointerEventData eventData)
			=> ShowHelper();

		public void OnPointerExit(PointerEventData eventData)
			=> HideHelper();

		private void ShowHelper()
		{
			_buttonSelectionTag.SetActive(true);

			Vector3 newPos = _buttonSelectionTag.transform.position;
			newPos.y = _buttonTransform.position.y;
			_buttonSelectionTag.transform.position = newPos;
		}

		private void HideHelper()
		{
			_buttonSelectionTag.SetActive(false);
		}
	}
}