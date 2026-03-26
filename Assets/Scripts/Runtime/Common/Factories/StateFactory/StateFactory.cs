using Runtime.Common.Services.StateMachine;
using Zenject;

namespace Runtime.Common.Factories.StateFactory
{
    /// <summary>
    /// Factory responsible for creating state instances (via the DI container).
    /// </summary>
    public class StateFactory
    {
        private IInstantiator _instantiator;

        [Inject]
        public StateFactory(IInstantiator instantiator) =>
            _instantiator = instantiator;

        public TState Create<TState>() where TState : IState =>
            _instantiator.Instantiate<TState>();
    }
}
