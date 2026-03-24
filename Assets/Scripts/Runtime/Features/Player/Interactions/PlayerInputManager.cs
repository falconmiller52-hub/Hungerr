using UnityEngine;
using NaughtyAttributes;
using Runtime.Features.Player.Interactions;
using Runtime.Features.Player.Movement;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    //Переменные инспектора

    //Внутренние переменные
    private bool _isJumping = false;

    //Кэшированные переменные
    PlayerInput _playerInput;
    PlayerStance _playerStance;
    PlayerCamera _playerCamera;
    PlayerMovement _playerMovement;
    PlayerScope _playerScope;
    PlayerFlashlight _playerFlashlight;
    PlayerInputActions _playerInputActions;

    //Методы Моно
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerStance = GetComponent<PlayerStance>();
        _playerCamera = GetComponent<PlayerCamera>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerScope = GetComponent<PlayerScope>();
        _playerFlashlight = GetComponent<PlayerFlashlight>();
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Player.Crouch.started += Crouch;

        _playerInputActions.Player.Run.started += Run;
        _playerInputActions.Player.Run.canceled += Run;

        _playerInputActions.Player.Jump.started += JumpStart;
        _playerInputActions.Player.Jump.canceled += JumpStop;

        _playerInputActions.Player.Flashlight.started += Flashlight;

        _playerInputActions.Player.Interact.started += Interact;

        _playerInputActions.Player.Enable();
    }

    private void Update()
    {
        if (_isJumping)
        {
            _playerMovement.Jump(_playerMovement.JumpHeight);
        }
    }

    private void FixedUpdate()
    {
        // _playerMovement.Move(MovingDirection);
    }

    private void LateUpdate()
    {

        // _playerCamera.LookAt(_playerCamera.CameraRotation);
    }

    //Методы скрипта
    private void Crouch(InputAction.CallbackContext context)
    {
        _playerStance.Crouch();
    }

    private void Run(InputAction.CallbackContext context)
    {
        // _playerStance.Run();
    }

    private void JumpStart(InputAction.CallbackContext context)
    {
        // _isJumping = true;
    }

    private void JumpStop(InputAction.CallbackContext context)
    {
        // _isJumping = false;
    }

    private void Flashlight(InputAction.CallbackContext context)
    {
        _playerFlashlight.Toggle();
    }

    private void Interact(InputAction.CallbackContext context)
    {
        // _playerScope.Interact();
    }

    //Геттеры и сеттеры
    public Vector2 MovingDirection
    {
        get
        {
            var inputAxis = _playerInputActions.Player.Move.ReadValue<Vector2>();
            var moveDirection = transform.forward * inputAxis.y + transform.right * inputAxis.x;
            var directionResult = new Vector2(moveDirection.x, moveDirection.z);
            return directionResult;
        }
    }
    public Vector2 MouseAxis => _playerInputActions.Player.Look.ReadValue<Vector2>() * Time.deltaTime;
}