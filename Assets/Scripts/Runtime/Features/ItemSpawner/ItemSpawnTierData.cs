using System.Collections.Generic;
using Runtime.Features.Inventory;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Features.ItemSpawner
{
	[CreateAssetMenu(fileName = "Item Spawn Tier", menuName = "Inventory/Item Spawn Tier")]
	public class ItemSpawnTierData : ScriptableObject
	{
		[SerializeField] private List<ItemWeight> _items;

		public InventoryItemData GetRandomItem()
		{
			int totalWeight = 0;

			foreach (var item in _items)
				totalWeight += item.Weight;

			int randomValue = Random.Range(0, totalWeight);
			int currentWeight = 0;

			foreach (var item in _items)
			{
				currentWeight += item.Weight;

				if (randomValue < currentWeight)
					return item.ItemData;
			}

			return null;
		}
	}
}