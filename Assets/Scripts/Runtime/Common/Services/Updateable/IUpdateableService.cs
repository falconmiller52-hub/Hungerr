using System;

namespace Runtime.Common.Services.Updateable
{
	public interface IUpdateableService : IDisposable
	{
		void AddUpdateable(IUpdateable updateable);
		void RemoveUpdateable(IUpdateable updateable);

	}
}
