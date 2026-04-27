namespace Runtime.Common.Services.SaveLoad
{
	[System.Serializable]
	public struct SlotSaveData
	{
		public SerializableVector2Int Position;
		public int ItemDataID;
		public int Amount;
	}
}