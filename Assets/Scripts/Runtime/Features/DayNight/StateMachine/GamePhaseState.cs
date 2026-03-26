using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Common.Services.StateMachine;

namespace Runtime.Features.DayNight.StateMachine
{
    public abstract class GamePhaseState : IState
    {
        protected readonly EventBus EventBus;
        protected readonly PhaseStateMachine Owner;
        protected readonly ILoadingCurtain Curtain;
        protected readonly IInputHandler InputHandler;

        protected GamePhaseState(PhaseStateMachine owner, EventBus eventBus, ILoadingCurtain curtain, IInputHandler inputHandler)
        {
            Owner = owner;
            EventBus = eventBus;
            Curtain = curtain;
            InputHandler = inputHandler;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
    }
}
