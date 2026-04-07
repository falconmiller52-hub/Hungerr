// InventoryManager.cs

using UnityEngine;
using Zenject;

namespace Runtime.Features.Inventory
{
	/// <summary>
	/// нахуй он нужен?
	/// по сути просто спавнит по запросу айтем в мире, никак не относится к инвентарю
	/// </summary>
	public class ItemWorldManager
	{
		// [Inject] private GameObject worldItemPrefab;
		//
  //   
		// public WorldItem SpawnItemInWorld(InventoryItemData data, int amount, Vector3 position, Quaternion rotation)
		// {
		// 	var item = Object.Instantiate(worldItemPrefab, position, rotation).GetComponent<WorldItem>();
		// 	item.Initialize(data, amount);
		// 	return item;
		// }
  //   
		// public WorldItem SpawnItemInWorld(InventoryItemData data, int amount, Vector3 position)
		// {
		// 	return SpawnItemInWorld(data, amount, position, Quaternion.identity);
		// }
	}
}