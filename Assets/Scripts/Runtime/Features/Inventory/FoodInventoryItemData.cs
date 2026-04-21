using UnityEngine;

namespace Runtime.Features.Inventory
{
	[CreateAssetMenu(fileName = "Food ItemData",  menuName = "Inventory/Food Item Data")]
	public class FoodInventoryItemData : InventoryItemData
	{
		public override EItemType ItemType => EItemType.Food;
	}
}