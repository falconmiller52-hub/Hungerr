using System.Collections.Generic;

namespace Runtime.Common.Services.SaveLoad
{
	[System.Serializable]
	public class GameStateData
	{
		public List<InventoryItemSaveData> PlayerInventoryItems = new();
		public List<InventoryItemSaveData> StorageInventoryItems = new();
		public List<ItemSpawnPointSaveData> SpawnPoints = new();
		public float Health;
		public int CurrentDay;
	}
}