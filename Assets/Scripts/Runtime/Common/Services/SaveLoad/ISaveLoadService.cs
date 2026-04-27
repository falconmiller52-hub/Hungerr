namespace Runtime.Common.Services.SaveLoad
{
	public interface ISaveLoadService
	{
		void SaveData(GameStateData data);
		GameStateData LoadData();
	}
}