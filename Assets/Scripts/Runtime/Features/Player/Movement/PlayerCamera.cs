using System.Collections.Generic;
using NaughtyAttributes;
using Runtime.Common.Services.Input;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Movement
{
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerStance))]
    public class PlayerCamera : MonoBehaviour
    {
        //Переменные инспектора
        [SerializeField, Label("Player Camera Objects Assign")] private List<GameObject> _cameraObjects;
        [SerializeField, Label("Player Transform Object")] private Transform _playerTransform;

        [Space, SerializeField, Label("Camera Sensitivity of X and Y axis")] private Vector2 _xySensitivity = Vector2.one;
        [SerializeField, Label("Minimum and Maximum Angle of Y axis"), MinMaxSlider(-90f, 90f)] private Vector2 _minMaxYAngle = Vector2.zero;

        [Space, SerializeField, Label("Breathing Magnitude")] private float _breathingMagnitude = 0f;
        [SerializeField, Label("Breathing Speed")] private float _breathingSpeed = 0f;

        [Space, SerializeField, Label("Stepping Magnitude")] private float _steppingMagnitude = 0f;
        [SerializeField, Label("Stepping Speed")] private float _steppingSpeed = 0f;

        [Space, SerializeField, Label("Camera FOV")] private float _cameraFov = 75f;
        [SerializeField, Label("Running FOV Damping")] private float _runFovDamp = 1f;
        [SerializeField, Label("Running FOV Multiplier")] private float _runFovMultiplier = 1f;

        //Внутренние переменные
        private float _xRotation, _yRotation;
        private Vector2 _mouseRotationInputDirection;
    
        //Кэшированные переменные
        private IInputHandler _inputHandler;
        private PlayerStance _playerStance;
        private Camera _camera;


        [Inject]
        private void Construct(IInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }
    
        private void Start()
        {
            _playerStance = GetComponent<PlayerStance>();
            _camera = _cameraObjects[4].GetComponent<Camera>();

            _xRotation = _cameraObjects[3].transform.localEulerAngles.x;
            _yRotation = transform.localEulerAngles.y;

            MouseToggle("-");
        }

        private void OnEnable()
        {
            if (_inputHandler == null)
            {
                Debug.LogError("PlayerCamera::OnEnable() No Input Handler was assigned");
                return;
            }
        
            _inputHandler.RotateInputChanged += SetNewMousePositionInput;
        }

        private void OnDisable()
        {
            if (_inputHandler == null)
            {
                Debug.LogError("PlayerCamera::OnDisable() No Input Handler was assigned");
                return;
            }
        
            _inputHandler.RotateInputChanged -= SetNewMousePositionInput;
        }

        private void Update()
        {
            BreatheMove();
            StepMove();
            FovChange();
        }

        private void LateUpdate()
        {
            LookAt(CameraRotation);
        }

        //Методы скрипта

        private void BreatheMove()
        {
            var nextBreathePosition = Vector3.up * Mathf.Sin(Time.time * _breathingSpeed) * _breathingMagnitude;

            _cameraObjects[1].transform.localPosition = nextBreathePosition;
        }

        private void StepMove()
        {
            var playerCurrentStance = _playerStance.CurrentStance;
            var playerCurrentSpeed = _playerStance.StanceSpeed(playerCurrentStance);

            var playerMovingDirectionMagnitude = _playerTransform.forward.magnitude;

            var nextStepPosition = Vector3.up * Mathf.Sin(Time.time * _steppingSpeed * playerCurrentSpeed) * _steppingMagnitude * playerMovingDirectionMagnitude;

            _cameraObjects[2].transform.localPosition = nextStepPosition;
        }

        private void FovChange()
        {
            var nextFov = _cameraFov * _runFovMultiplier;
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _playerStance.CurrentStance == PlayerStance.Stance.Running && _playerTransform.forward.magnitude > 0f ? nextFov : _cameraFov, Time.deltaTime * _runFovDamp);
        }

        public void LookAt(Vector2 direction)
        {
            transform.localEulerAngles = new Vector3(0, direction.y, 0);

            var angleChange = direction.x;

            if (angleChange > 180f) angleChange -= 360f;
            angleChange = Mathf.Clamp(angleChange, _minMaxYAngle.x, _minMaxYAngle.y);

            _cameraObjects[3].transform.localEulerAngles = new Vector3(angleChange, 0, 0);
        }

        public void MouseToggle(string state = "~")
        {
            bool visible = Cursor.visible;
            CursorLockMode lockState = Cursor.lockState;

            if (state == "~")
            {
                visible = !visible;
                lockState = lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
            }
            else if (state == "-")
            {
                visible = false;
                lockState = CursorLockMode.Locked;
            }
            else
            {
                visible = true;
                lockState = CursorLockMode.None;
            }

            Cursor.visible = visible; Cursor.lockState = lockState;
        }

        private void SetNewMousePositionInput(Vector2 input)
        {
            _mouseRotationInputDirection = input;
        }

        private void CalculateRotation()
        {
            // Убираем лишние умножения, оставляем только одно для плавности
            float mouseX = _mouseRotationInputDirection.x * _xySensitivity.x * Time.deltaTime;
            float mouseY = _mouseRotationInputDirection.y * _xySensitivity.y * Time.deltaTime;

#if UNITY_EDITOR
            mouseX *= 10f; // Оставляем твой фикс для редактора, если он нужен
            mouseY *= 10f;
#endif

            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, _minMaxYAngle.x, _minMaxYAngle.y);
        }
        
        //Геттеры и сеттеры
        private Vector2 CameraRotation
        {
            get
            {
                _mouseRotationInputDirection *= Time.deltaTime;
#if UNITY_EDITOR
                _mouseRotationInputDirection *= 10f;
#endif
    
                var yAxis = _mouseRotationInputDirection.x;
                var xAxis = _mouseRotationInputDirection.y;
    
                _yRotation += yAxis * _xySensitivity.x * Time.deltaTime;
    
                _xRotation -= xAxis * _xySensitivity.y * Time.deltaTime;
                _xRotation = Mathf.Clamp(_xRotation, _minMaxYAngle.x, _minMaxYAngle.y);
    
                return new Vector2(_xRotation, _yRotation);
            }
        }
        public List<GameObject> CameraObjects
        {
            get => _cameraObjects;
            set => _cameraObjects = value;
        }

        public Vector2 Sensitivity
        {
            get => _xySensitivity;
            set => _xySensitivity = value;
        }

        public Vector2 MinMaxYAngle
        {
            get => _minMaxYAngle;
            set => _minMaxYAngle = value;
        }

        public float BreathingMagnitude
        {
            get => _breathingMagnitude;
            set => _breathingMagnitude = value;
        }

        public float BreathingSpeed
        {
            get => _breathingSpeed;
            set => _breathingSpeed = value;
        }

        public float SteppingMagnitude
        {
            get => _steppingMagnitude;
            set => _steppingMagnitude = value;
        }

        public float SteppingSpeed
        {
            get => _steppingSpeed;
            set => _steppingSpeed = value;
        }

        public float CameraFOV
        {
            get => _cameraFov;
            set => _cameraFov = value;
        }

        public float RunningFOVDamp
        {
            get => _runFovDamp;
            set => _runFovDamp = value;
        }

        public float RunningFOVMultiplier
        {
            get => _runFovMultiplier;
            set => _runFovMultiplier = value;
        }
    }
}