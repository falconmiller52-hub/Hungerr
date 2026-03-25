using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.UI
{
	public class MenuButtonHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private GameObject _buttonSelectionTag;
		[SerializeField] private RectTransform _buttonTransform;
		
		public void OnPointerEnter(PointerEventData eventData)
		{
			_buttonSelectionTag.SetActive(true);
			
			Vector3 newPos = _buttonSelectionTag.transform.position;
			newPos.y = _buttonTransform.position.y; 
			_buttonSelectionTag.transform.position = newPos;
		}
		
		public void OnPointerExit(PointerEventData eventData)
		{
			_buttonSelectionTag.SetActive(false);
		}
	}
}