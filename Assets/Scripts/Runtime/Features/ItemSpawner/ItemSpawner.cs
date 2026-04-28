using System.Collections.Generic;
using System.Linq;
using Runtime.Common.Services.ItemsIdentifier;
using Runtime.Features.Inventory;
using Runtime.Features.Inventory.WorldItem;
using UnityEngine;
using Zenject;

namespace Runtime.Features.ItemSpawner
{
	/// <summary>
	/// Класс отвечающий за спавн предметов в мире по вызову метода
	/// </summary>
	public class ItemSpawner : MonoBehaviour
	{
		private ItemSpawnPoint[] _spawnPoints;

		// <int - InstanceId of spawnPoint, int - ID in InventoryItemData >
		private Dictionary<WorldItem, int> _spawnPointsMap;
		private ItemsIdentifierSO _itemsIdentifier;
		private bool _isInited = false;
		
		[Inject]
		private void Construct(ItemsIdentifierSO itemsIdentifierSO)
		{
			_itemsIdentifier = itemsIdentifierSO;
		}

		/// <summary>
		/// спавнит айтемы на спавн поинтах
		/// </summary>
		[ContextMenu("Spawn Items")]
		public void SpawnItems()
		{
			if (_isInited)
				return;

			_spawnPointsMap = new Dictionary<WorldItem, int>();
			FindAllSpawnPoints();
			
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

					var worldItem = spawnedItem.GetComponent<WorldItem>();
					_spawnPointsMap.Add(worldItem, point.ID);

					worldItem.WorldItemDestroyed += OnWorldItemDestroyed;
				}
			}
			
			_isInited = true;
		}

		/// <summary>
		/// спавнит айтемы с загруженной даты
		/// </summary>
		/// <param name="spawnPointsData">int - pointId, int - item config id</param>
		public void SpawnItems(Dictionary<int, int> spawnPointsData)
		{
			if (_isInited)
				return;
			
			_spawnPointsMap = new Dictionary<WorldItem, int>();
			FindAllSpawnPoints();
			
			foreach (var point in _spawnPoints)
			{
				if (!spawnPointsData.TryGetValue(point.ID, out var value))
					continue;

				var selectedItem = _itemsIdentifier.GetItemDataById(value);

				if (selectedItem != null && selectedItem.WorldPrefab != null)
				{
					GameObject spawnedItem = Instantiate(selectedItem.WorldPrefab, point.transform.position, Quaternion.identity);

					spawnedItem.transform.localScale = point.ItemScale;

					Collider itemCol = spawnedItem.GetComponentInChildren<Collider>();
					float y = spawnedItem.transform.position.y + (itemCol.bounds.extents.y / 2);
					Vector3 position = new Vector3(point.transform.position.x, y, point.transform.position.z);

					spawnedItem.transform.position = position;
					
					var worldItem = spawnedItem.GetComponent<WorldItem>();
					_spawnPointsMap.Add(worldItem, point.ID);

					worldItem.WorldItemDestroyed += OnWorldItemDestroyed;
				}
			}
			
			_isInited = true;
		}

		public Dictionary<int, int> GetSpawnPointsData()
		{
			return _spawnPointsMap.ToDictionary(
				kvp => kvp.Value, 
				kvp => kvp.Key.GetItem().Data.Id);
		}

		private void FindAllSpawnPoints()
		{
			_spawnPoints = FindObjectsByType<ItemSpawnPoint>(FindObjectsSortMode.None);
		}
		
		private void OnWorldItemDestroyed(WorldItem worldItem)
		{
			_spawnPointsMap.Remove(worldItem);
			worldItem.WorldItemDestroyed -= OnWorldItemDestroyed;
		}
	}
}