using Runtime.Common.Factories.StateFactory;
using Runtime.Infra.GameplayScene.GameplayStateMachine;
using Runtime.Infra.GameplayScene.GameplayStateMachine.States;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.GameplayScene
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        private SceneStateMachine _stateMachine;
        private StateFactory _stateFactory;

        [Inject]
        public void Construct(SceneStateMachine stateMachine, StateFactory stateFactory)
        {
            _stateMachine = stateMachine;
            _stateFactory = stateFactory;
        }

        private void Awake()
        {
            _stateMachine.RegisterState(_stateFactory.Create<EntryGameplayState>());
            _stateMachine.RegisterState(_stateFactory.Create<PlayGameplayState>());
            _stateMachine.RegisterState(_stateFactory.Create<ExitGameplayState>());

            _stateMachine.EnterIn<EntryGameplayState>();
        }
    }
}
