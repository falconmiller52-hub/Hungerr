using System.Collections.Generic;

namespace Runtime.Common.Services.SaveLoad
{
	[System.Serializable]
	public class GameStateData
	{
		
		public List<SlotSaveData> Slots = new List<SlotSaveData>();
		public float Health;
	}
}