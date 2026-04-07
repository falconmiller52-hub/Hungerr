// ItemDragger.cs

using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Runtime.Features.Inventory
{
	/// <summary>
	/// хендлит то если игрок мышью попытается взять предмет при открытом инвентаре
	/// мб должен лежать как компонент вместе с инвентарем
	/// </summary>
	public class ItemDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[SerializeField] private InventoryUIView inventoryView;
    
		private InventoryItem draggingItem;
		private Vector2 startPosition;
		private Transform parentsBeforeDrag;
		private Canvas canvas;
    
		[Inject] private PlayerInventory playerInventory;
    
		private void Awake()
		{
			canvas = GetComponentInParent<Canvas>();
		}
    
		public void OnBeginDrag(PointerEventData eventData)
		{
			var slotData = GetComponentInParent<InventorySlotView>();
			if (slotData == null || slotData.ItemInstance == null)
				return;
        
			draggingItem = slotData.ItemInstance;
			startPosition = transform.position;
			parentsBeforeDrag = transform.parent;
        
			transform.SetParent(canvas.transform);
			transform.SetAsLastSibling();
		}
    
		public void OnDrag(PointerEventData eventData)
		{
			transform.position = Input.mousePosition;
		}
    
		public void OnEndDrag(PointerEventData eventData)
		{
			transform.SetParent(parentsBeforeDrag);
        
				// хуйня, переделать, статик логика
			var hit = InventorySlotView.GetSlotUnderCursor();
			if (hit != null && inventoryView != null)
			{
				var targetSlot = hit.SlotPosition;
				if (playerInventory.GetInventory().CanPlaceItem(draggingItem, targetSlot))
				{
					playerInventory.GetInventory().MoveItem(draggingItem, targetSlot);
					inventoryView.Refresh();
				}
			}
        
			draggingItem = null;
		}
	}
}