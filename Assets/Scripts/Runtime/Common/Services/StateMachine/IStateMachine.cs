namespace Runtime.Common.Services.StateMachine
{
	public interface IStateMachine
	{
		void EnterIn<TState>() where TState : IState;
		void RegisterState<TState>(TState state) where TState : IState;
	}
}