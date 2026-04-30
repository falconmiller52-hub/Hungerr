#if UNITY_EDITOR
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

			// Clean path: trim spaces and slashes
			string cleanPath = relativePath.Replace("\\", "/").Trim('/');
			string folderPath = $"Assets/{cleanPath}";

			// Check folder existence via AssetDatabase
			if (!AssetDatabase.IsValidFolder(folderPath))
			{
				Debug.LogError($"[EditorFinder] Папка не найдена по пути: {folderPath}");
				return result;
			}

			// Search: t:TypeName looks for all files of this class (and descendants) in the given paths
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
#endif