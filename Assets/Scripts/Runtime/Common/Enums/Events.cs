namespace Runtime.Common.Enums
{
    /// <summary>
    /// ивент о глобальной смене состояния
    /// </summary>
    public enum GameEvent
    {
        StartGameplay = 0,
        EndGameplay,
        QuitGame
    }
    
    /// <summary>
    /// ивент о старте геймплейной фазы (дня или ночи)
    /// </summary>
    public enum GameplayStateEvent
    {
        StartNightPhaseTrigger,
        EndNightPhaseTrigger,
    }
}