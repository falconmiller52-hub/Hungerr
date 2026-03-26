using UnityEngine;

namespace Runtime.Common.Services.ResourceLoad
{
	public interface IResourceLoader
	{
		T Load<T>(string path) where T : Object;
		void UnloadUnused();
	}
}
