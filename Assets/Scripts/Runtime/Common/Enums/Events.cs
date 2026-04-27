namespace Runtime.Common.Enums
{
	/// <summary>
	/// ивент о глобальной смене состояния
	/// </summary>
	public enum EGameEvent
	{
		StartGameplay = 0,
		EndGameplay,
		QuitGame
	}

	/// <summary>
	/// ивент о триггере для смены геймплейной фазы (дня или ночи)
	/// </summary>
	public enum EGameplayChangePhaseTriggerEvent
	{
		StartNightTrigger = 0,
		StartDayTrigger,
	}
	
	/// <summary>
	/// ивент о смене новой геймплейной фазы (дня или ночи)
	/// </summary>
	public enum EGameplayChangedPhaseEvent
	{
		NightStarted = 0,
		DayStarted,
	}

	public enum EPlayerStanceEvent
	{
		StartWalkState = 0,
		StartRunState,
		StartCrouchState
	}
	
	public enum EDomovoiSatietyLevel
	{
		Normal = 0,
		Critical
	}

	public enum EGameOver
	{
		PlayerOnZeroHealth
	}
}