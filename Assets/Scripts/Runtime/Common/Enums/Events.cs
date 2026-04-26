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
	public enum EGameplayChangeStateTriggerEvent
	{
		StartNightPhaseTrigger,
		EndNightPhaseTrigger,
	}
	
	/// <summary>
	/// ивент о смене новой геймплейной фазы (дня или ночи)
	/// </summary>
	public enum EGameplayChangedStateEvent
	{
		OnStartNightPhase,
		OnEndNightPhase,
	}

	public enum EPlayerStanceEvent
	{
		StartWalkState,
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