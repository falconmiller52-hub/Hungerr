using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(PlayerCamera))]
public class PlayerFlashlight : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Flashlight Object")] private Light _flashlightObject;
    [SerializeField, Label("Flashlight Intensity")] private float _intensity = 1f;

    //Внутренние переменные


    //Кэшированные переменные

    //Методы Моно

    //Методы скрипта
    public void Toggle(string state = "~")
    {
        var initialIntensity = _flashlightObject.intensity;

        if (state == "~")
        {
            if (initialIntensity == 0f) initialIntensity = _intensity;
            else initialIntensity = 0f;   
        }
        else if (state == "+")
        {
            initialIntensity = _intensity;
        }
        else
        {
            initialIntensity = 0f;
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