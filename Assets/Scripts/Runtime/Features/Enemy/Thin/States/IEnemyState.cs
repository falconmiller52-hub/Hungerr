namespace Runtime.Features.Enemy.Thin.States
{
	public interface IEnemyState
	{
		void Enter();       // Вызывается один раз при входе в состояние
		void Execute();     // Вызывается каждый кадр (как Update)
		void Exit();        // Вызывается перед переходом в другое состояние
	}
}