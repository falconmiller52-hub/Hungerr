using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Runtime.Common.Services.SaveLoad
{
	public class SaveLoadService : ISaveLoadService
	{
		string _filePath;

		public SaveLoadService()
		{
			_filePath = Application.persistentDataPath + "/save.gamesave";
		}

		public void SaveData(GameStateData data)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(_filePath, FileMode.Create);

			binaryFormatter.Serialize(fileStream, data);
			fileStream.Close();

			Debug.Log("Game saved at: " + _filePath);
		}

		public GameStateData LoadData()
		{
			if (!File.Exists(_filePath))
			{
				Debug.LogError("Save file not found!");
				return null;
			}

			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(_filePath, FileMode.Open);

			GameStateData gameStateData = (GameStateData)binaryFormatter.Deserialize(fileStream);
			fileStream.Close();

			return gameStateData;
		}
	}
}