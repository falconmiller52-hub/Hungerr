namespace Runtime.Common.Services.StateMachine
{
	public interface IState
	{
		void Enter();
		void Execute(); 
		void Exit();
	}
}