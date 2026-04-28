using System.Collections.Generic;

namespace Runtime.Common.Services.SaveLoad
{
	[System.Serializable]
	public class GameStateData
	{
		public List<InventoryItemSaveData> Slots = new List<InventoryItemSaveData>();
		public List<ItemSpawnPointSaveData> SpawnPoints = new List<ItemSpawnPointSaveData>();
		public float Health;
	}
}