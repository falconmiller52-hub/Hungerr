using Runtime.Common.Constants;
using Runtime.Common.Factories.StateFactory;
using Runtime.Common.Helpers;
using Runtime.Infra.App.GlobalStateMachine.States;
using Zenject;

namespace Runtime.Infra.App
{
    public class GlobalEntryPoint : IInitializable
    {
        private GlobalStateMachine.GlobalStateMachine _globalStateMachine;
        private StateFactory _stateFactory;

        [Inject]
        public void Construct(GlobalStateMachine.GlobalStateMachine gameStateMachine, StateFactory stateFactory)
        {
            _globalStateMachine = gameStateMachine;
            _stateFactory = stateFactory;
        }

        public void Initialize()
        {
            _globalStateMachine.RegisterState(_stateFactory.Create<GameBootstrapState>());
            _globalStateMachine.RegisterState(_stateFactory.Create<GameMenuState>());
            _globalStateMachine.RegisterState(_stateFactory.Create<GameplayLoopState>());
            _globalStateMachine.RegisterState(_stateFactory.Create<GameExitState>());

            if (!QuickStartBridge.IsQuickStart)
            {
                _globalStateMachine.EnterIn<GameBootstrapState>();
            }
            else
            {
                if (QuickStartBridge.SceneName == Scenes.MenuName)
                    _globalStateMachine.EnterIn<GameMenuState>();
                else
                    _globalStateMachine.EnterIn<GameplayLoopState>(); 
            }
        }
    }
}
