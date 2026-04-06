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
	/// ивент о старте геймплейной фазы (дня или ночи)
	/// </summary>
	public enum EGameplayStateEvent
	{
		StartNightPhaseTrigger,
		EndNightPhaseTrigger,
	}
}