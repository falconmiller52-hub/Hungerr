using Runtime.Common.Services.Input;
using UnityEngine;
using Zenject;

namespace Runtime.Common.EntryPoints
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        private InputHandler _inputHandler;

        [Inject]
        private void Construct(InputHandler inputHandler)
        {
            _inputHandler = inputHandler;;
        }

        private void Start()
        {
            _inputHandler.Enable();
        }
    }
}
