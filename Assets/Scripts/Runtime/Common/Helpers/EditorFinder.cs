using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Runtime.Common.Helpers
{
	public class EditorFinder
	{
		private static EditorFinder _instance;

		public static EditorFinder Instance => _instance ??= new EditorFinder();

		/// <summary>
		/// Finds all assets of type T in a folder at a relative path from Assets/
		/// </summary>
		public List<T> GetAssetsFromFolder<T>(string relativePath) where T : Object
		{
			List<T> result = new List<T>();

			if (string.IsNullOrEmpty(relativePath))
			{
				Debug.LogWarning("[EditorFinder] Путь пуст.");
				return result;
			}

			// Чистим путь: убираем лишние пробелы и слеши в начале/конце
			string cleanPath = relativePath.Replace("\\", "/").Trim('/');
			string folderPath = $"Assets/{cleanPath}";

			// Проверка существования папки через AssetDatabase
			if (!AssetDatabase.IsValidFolder(folderPath))
			{
				Debug.LogError($"[EditorFinder] Папка не найдена по пути: {folderPath}");
				return result;
			}

			// Поиск: t:Тип ищет все файлы этого класса (и наследников) в указанном массиве путей
			string filter = $"t:{typeof(T).Name}";
			string[] guids = AssetDatabase.FindAssets(filter, new[] { folderPath });

			foreach (var guid in guids)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);
				T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

				if (asset != null)
				{
					result.Add(asset);
				}
			}

			return result;
		}

		/// <summary>
		/// Finds ALL assets of type T in the entire project (Assets folder).
		/// </summary>
		public List<T> GetAllAssetsByType<T>() where T : Object
		{
			List<T> result = new List<T>();

			// Фильтр "t:TypeName" заставляет Unity искать объекты этого типа в глобальном индексе
			string filter = $"t:{typeof(T).Name}";

			// Передаем null вторым аргументом, чтобы искать по всему проекту
			string[] guids = AssetDatabase.FindAssets(filter);

			foreach (var guid in guids)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);
				T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

				if (asset != null)
				{
					result.Add(asset);
				}
			}

			return result;
		}
	}
}