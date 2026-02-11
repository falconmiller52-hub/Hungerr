using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(PlayerCamera))]
public class PlayerFlashlight : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Flashlight Object")] private GameObject _flashlightObject;
    [SerializeField, Label("Flashlight Smoothness")] private float _flashlightDamp = 1f;

    //Внутренние переменные


    //Кэшированные переменные
    PlayerCamera _playerCamera;

    //Методы Моно
    private void Start()
    {
        _playerCamera = GetComponent<PlayerCamera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) Toggle();

        FlashlightMove();
    }

    //Методы скрипта
    private void FlashlightMove()
    {
        var xRotation = -_playerCamera.MouseAxis.x * _playerCamera.Sensitivity.x * Time.deltaTime * _flashlightDamp;
        var yRotation = -_playerCamera.MouseAxis.y * _playerCamera.Sensitivity.y * Time.deltaTime * _flashlightDamp;

        var vectorResult = new Vector3(yRotation, xRotation, 0);

        _flashlightObject.transform.localEulerAngles = vectorResult;
    }

    public void Toggle(string state = "~")
    {
        bool toggleState = _flashlightObject.activeSelf;

        if (state == "~")
        {
            toggleState = !toggleState;
        }
        else if (state == "+")
        {
            toggleState = true;
        }
        else
        {
            toggleState = false;
        }

        _flashlightObject.SetActive(toggleState);
    }

    //Геттеры и сеттеры
}