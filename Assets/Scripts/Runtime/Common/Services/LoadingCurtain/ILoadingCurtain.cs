using System;

namespace Runtime.Common.Services.LoadingCurtain
{
	public interface ILoadingCurtain
	{
		void Show(float customTime = -1, bool needText = true, Action onEnd = null);
		void Hide(float customTime = -1, bool needText = true, Action onEnd = null);
	}
}