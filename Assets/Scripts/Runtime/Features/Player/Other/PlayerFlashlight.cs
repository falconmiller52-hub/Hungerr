using NaughtyAttributes;
using Runtime.Common.Services.Input;
using Runtime.Features.Player.Movement;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Other
{
    [RequireComponent(typeof(PlayerCamera))]
    public class PlayerFlashlight : MonoBehaviour
    {
        //Переменные инспектора
        [SerializeField, Label("Flashlight Object")] private Light _flashlightObject;
        [SerializeField, Label("Flashlight Intensity")] private float _intensity = 1f;

        [Space, SerializeField, Label("Flashlight ON Sound Object")] private AudioSource _flashlightsSoundObjectOn;
        [SerializeField, Label("Flashlight OFF Sound Object")] private AudioSource _flashlightsSoundObjectOff;
    
        private IInputHandler _inputHandler;
        private bool _isEnabled = false;


        [Inject]
        private void Construct(IInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }
    
        private void OnEnable()
        {
            if (_inputHandler == null)
            {
                Debug.LogError("PlayerFlashlight::OnEnable() No Input Handler was assigned");
                return;
            }
        
            _inputHandler.FlashlightInputPressed += Toggle;
        }

        private void OnDisable()
        {
            if (_inputHandler == null)
            {
                Debug.LogError("PlayerFlashlight::OnDisable() No Input Handler was assigned");
                return;
            }
        
            _inputHandler.FlashlightInputPressed -= Toggle;
        }
        
        public void Toggle()
        {
            var initialIntensity = _flashlightObject.intensity;

            if (_isEnabled == false)
            {
                if (initialIntensity == 0f)
                {
                    initialIntensity = _intensity;
                    _flashlightsSoundObjectOn.Play();
                }
                else
                {
                    initialIntensity = 0f;
                    _flashlightsSoundObjectOff.Play();
                }

                _isEnabled = true;
            }
            else
            {
                initialIntensity = 0f;
                _flashlightsSoundObjectOff.Play();
                _isEnabled = false;
            }

            _flashlightObject.intensity = initialIntensity;
        }

        //Геттеры и сеттеры
        public Light Object
        {
            get => _flashlightObject;
            set => _flashlightObject = value;
        }

        public float Intensity
        {
            get => _intensity;
            set => _intensity = value;
        }
    }
}