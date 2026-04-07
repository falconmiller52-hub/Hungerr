// InventorySlotView.cs

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.Features.Inventory
{
	public class InventorySlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private TMP_Text _amountText;
		[SerializeField] private Transform _parent;
    
		private GameObject model;
		private InventoryItem itemInstance;
		private Vector2Int slotPosition;
    
		private static InventorySlotView slotUnderCursor;
    
		public InventoryItem ItemInstance => itemInstance;
		public Vector2Int SlotPosition => slotPosition;
		
    
		public void Initialize(Vector2Int position, InventoryItem item = null)
		{
			slotPosition = position;
			SetItem(item);
		}
    
		public void SetItem(InventoryItem item)
		{
			if (item == null)
			{
				itemInstance = null;
				_amountText.enabled = false;
				if (model != null)
				{
					Destroy(model);
					model = null;
				}

				return;
			}

			if (item == itemInstance)
				return;
			
			itemInstance = item;
			
			_amountText.text = item._amount > 1 ? item._amount.ToString() : "";
			_amountText.enabled = item._amount > 1;
			
			var d = Instantiate(itemInstance._data.PrefabForInventory, Vector3.zero, Quaternion.identity);
			d.transform.SetParent(_parent);
			d.transform.localPosition = Vector3.zero;
			d.transform.localScale = itemInstance._data.Scale;

			model = d;
		}
    
		public static InventorySlotView GetSlotUnderCursor() => slotUnderCursor;
    
		public void OnPointerEnter(PointerEventData eventData)
		{
			slotUnderCursor = this;
		}
    
		public void OnPointerExit(PointerEventData eventData)
		{
			if (slotUnderCursor == this)
				slotUnderCursor = null;
		}
	}
}