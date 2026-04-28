using System.Collections.Generic;

namespace Runtime.Common.Services.SaveLoad
{
	[System.Serializable]
	public class GameStateData
	{
		public List<InventoryItemSaveData> PlayerInventoryItems = new List<InventoryItemSaveData>();
		public List<InventoryItemSaveData> StorageInventoryItems = new List<InventoryItemSaveData>();
		public List<ItemSpawnPointSaveData> SpawnPoints = new List<ItemSpawnPointSaveData>();
		public float Health;
	}
}