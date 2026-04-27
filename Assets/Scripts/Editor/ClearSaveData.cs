using UnityEditor;
using UnityEngine;
using System.IO;

public class SaveSystemEditor
{
	// Меню появится в верхней панели Unity под названием "Game Tools"
	[MenuItem("Game Tools/Clear Save Data")]
	public static void ClearSaveData()
	{
		// Путь должен совпадать с тем, что и в SaveLoadService
		string filePath = Application.persistentDataPath + "/save.gamesave";

		if (File.Exists(filePath))
		{
			File.Delete(filePath);
			Debug.Log($"<color=green>Save file deleted successfully at:</color> {filePath}");
			
		}
		else
		{
			Debug.LogWarning("No save file found to delete.");
		}
	}

	// Добавляем возможность быстро открыть папку с сохранением (очень полезно)
	[MenuItem("Game Tools/Open Save Folder")]
	public static void OpenSaveFolder()
	{
		EditorUtility.RevealInFinder(Application.persistentDataPath);
	}
}