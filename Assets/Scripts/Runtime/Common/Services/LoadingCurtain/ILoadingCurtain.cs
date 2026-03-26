namespace Runtime.Common.Services.LoadingCurtain
{
	public interface ILoadingCurtain
	{
		void Show(float customTime = -1, bool needText = true);
		void Hide(float customTime = -1, bool needText = true);
	}
}