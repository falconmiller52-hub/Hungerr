using UnityEngine;

namespace Runtime.Features.Inventory.ItemSpawner
{
	/// <summary>
	/// Класс отвечающий за спавн предметов в мире по вызову метода
	/// </summary>
	public class ItemSpawner : MonoBehaviour
	{
		private ItemSpawnPoint[] _spawnPoints;

		private void Start()
		{
			_spawnPoints = FindObjectsByType<ItemSpawnPoint>(FindObjectsSortMode.None);
		}

		[ContextMenu("Spawn Items")]
		public void SpawnItems()
		{
			foreach (var point in _spawnPoints)
			{
				if (point.TierData == null)
					continue;

				InventoryItemData selectedItem = point.TierData.GetRandomItem();

				if (selectedItem != null && selectedItem.WorldPrefab != null)
				{
					GameObject spawnedItem = Instantiate(selectedItem.WorldPrefab, point.transform.position, Quaternion.identity);

					spawnedItem.transform.localScale = point.ItemScale;

					Collider itemCol = spawnedItem.GetComponentInChildren<Collider>();
					float y = spawnedItem.transform.position.y + (itemCol.bounds.extents.y / 2);
					Vector3 position = new Vector3(point.transform.position.x, y, point.transform.position.z);

					spawnedItem.transform.position = position;
				}
			}
		}
	}
}