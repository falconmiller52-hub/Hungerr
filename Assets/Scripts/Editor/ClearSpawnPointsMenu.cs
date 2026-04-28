using UnityEngine;
using UnityEditor;
using Runtime.Common.Services.SaveLoad;

public class ClearSpawnPointsMenu : Editor
{
	[MenuItem("Game Tools/Clear SpawnPoints from Save", false, 1)]
	static void ClearSpawnPoints()
	{
		SaveLoadService service = new SaveLoadService();
		GameStateData data = service.LoadData();
        
		if (data == null)
		{
			Debug.LogWarning("No save file found!");
			return;
		}
        
		// Очищаем только SpawnPoints
		data.SpawnPoints = null;
        
		// Сохраняем обратно
		service.SaveData(data);
		Debug.Log("Save updated - SpawnPoints cleared!");
	}
}