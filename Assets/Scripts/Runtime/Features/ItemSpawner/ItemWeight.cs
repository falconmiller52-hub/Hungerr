using System;
using Runtime.Features.Inventory;

namespace Runtime.Features.ItemSpawner
{
	/// <summary>
	/// Дата о весе какого-то предмета относительно других в рамках Tier
	/// </summary>
	[Serializable]
	public struct ItemWeight
	{
		public InventoryItemData ItemData;
		public int Weight;
	}
}