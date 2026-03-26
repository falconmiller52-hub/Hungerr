namespace Runtime.Common.Services.LoadingCurtain
{
	public interface ILoadingCurtain
	{
		void Show();
		void Hide(float customTime = -1);
	}
}