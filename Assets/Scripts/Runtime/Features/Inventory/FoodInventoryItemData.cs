using UnityEngine;

namespace Runtime.Features.Inventory
{
	[CreateAssetMenu(fileName = "Food ItemData",  menuName = "Inventory/Food Item Data")]
	public class FoodInventoryItemData : InventoryItemData
	{
		[field: SerializeField] public int Satiety { get; private set; } = 1;
	}
}