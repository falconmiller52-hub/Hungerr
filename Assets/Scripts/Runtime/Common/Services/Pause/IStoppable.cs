namespace Runtime.Common.Services.Pause
{
	public interface IPausable
	{
		void Stop();
		void Resume();
	}
}