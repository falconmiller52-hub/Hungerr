using NaughtyAttributes;
using Runtime.Common.Services.Input;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Interactions
{
    public class PlayerScope : MonoBehaviour
    {
        //Переменные инспектора
        [SerializeField, Label("Player Camera")] private Camera _playerCamera;

        [Space, SerializeField, Label("Raycasting Length")] private float _rayLength;

        //Внутренние переменные
        private RaycastHit _rayHit;
        private GameObject _interactableObject;
        private ObjectInteraction _interaction;
        private IInputHandler _inputHandler;

        //Кэшированные переменные


        [Inject]
        private void Construct(IInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }
    
        private void OnEnable()
        {
            if (_inputHandler == null)
            {
                Debug.LogError("PlayerScope::OnEnable() No Input Handler was assigned");
                return;
            }
        
            _inputHandler.InteractPerformed += Interact;
        }

        private void OnDisable()
        {
            if (_inputHandler == null)
            {
                Debug.LogError("PlayerScope::OnDisable() No Input Handler was assigned");
                return;
            }
        
            _inputHandler.InteractPerformed -= Interact;
        }
    
        private void Update()
        {
            var ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Physics.Raycast(ray, out _rayHit, _rayLength);

            if (_rayHit.collider && _rayHit.collider.gameObject != _interactableObject && _rayHit.collider.gameObject.TryGetComponent<ObjectInteraction>(out ObjectInteraction _oi))
            {
                _interactableObject = _rayHit.collider.gameObject;

                _interaction = _interactableObject.GetComponent<ObjectInteraction>();
                _interaction.CurrentOutlineWidth = 25f;
            }
            else if ((!_rayHit.collider || _rayHit.collider && _rayHit.collider.gameObject != _interactableObject) && _interactableObject) 
            {
                _interaction.CurrentOutlineWidth = 0f;

                _interaction = null;
                _interactableObject = null;
            } 
        }

        //Методы скрипта
        private void Interact()
        {
            if (_interaction) 
                _interaction.InteractionEvents.Invoke();
        }

        //Геттеры и сеттеры


    }
}