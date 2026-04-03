using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Common.Services.ResourceLoad
{
	public class ResourceLoader : IResourceLoader
	{
		private readonly Dictionary<string, Object> _cachedResources = new();

		public T Load<T>(string path = "") where T : Object
		{
			if (_cachedResources.TryGetValue(path, out var resource))
			{
				return resource as T;
			}

			T loadedResource = Resources.Load<T>(path);

			if (loadedResource == null)
			{
				Debug.LogError($"ResourcesLoader::Load() Не удалось найти ресурс по пути: {path}");
				return null;
			}

			_cachedResources.Add(path, loadedResource);
			return loadedResource;
		}
		
		public void UnloadUnused()
		{
			_cachedResources.Clear();
			Resources.UnloadUnusedAssets();
			System.GC.Collect();
		}
	}
}
