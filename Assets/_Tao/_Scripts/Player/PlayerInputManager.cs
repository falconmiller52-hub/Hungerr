using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerStance))]
public class PlayerInputManager : MonoBehaviour
{
    //Переменные инспектора


    //Внутренние переменные


    //Кэшированные переменные
    PlayerInput _playerInput;
    PlayerStance _playerStance;
    PlayerInputActions _playerInputActions;

    //Методы Моно

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerStance = GetComponent<PlayerStance>();
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Player.Crouch.started += Crouch;
        _playerInputActions.Player.Run.started += Run;
        _playerInputActions.Player.Run.canceled += Run;

        _playerInputActions.Player.Enable();
    }

    //Методы скрипта
    private void Crouch(InputAction.CallbackContext context)
    {
        _playerStance.Crouch();
    }

    private void Run(InputAction.CallbackContext context)
    {
        _playerStance.Run();
    }

    //Геттеры и сеттеры
}