using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.LoadingCurtain;
using Runtime.Common.Services.StateMachine;
using Runtime.Features.Location;

namespace Runtime.Features.DayNight.StateMachine
{
    public abstract class GamePhaseState : IState
    {
        protected readonly EventBus EventBus;
        protected readonly PhaseStateMachine Owner;
        protected readonly ILoadingCurtain Curtain;
        protected readonly IInputHandler InputHandler;
        protected readonly LocationChanger LocationChanger;

        protected GamePhaseState(PhaseStateMachine owner, EventBus eventBus, ILoadingCurtain curtain, 
            IInputHandler inputHandler, LocationChanger locationChanger)
        {
            Owner = owner;
            EventBus = eventBus;
            Curtain = curtain;
            InputHandler = inputHandler;
            LocationChanger = locationChanger;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
    }
}
