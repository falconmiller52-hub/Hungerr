using System;
using UnityEngine;

namespace Runtime.Features.Inventory
{
	[Serializable]
	[CreateAssetMenu(fileName = "Food ItemData",  menuName = "Inventory/Food Item Data")]
	public class FoodInventoryItemData : InventoryItemData
	{
		[field: SerializeField] public int Satiety { get; private set; } = 1;
	}
}