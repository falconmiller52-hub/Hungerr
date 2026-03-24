using UnityEngine;
using NaughtyAttributes;
using Runtime.Features.Player.Movement;

[RequireComponent(typeof(PlayerCamera))]
public class PlayerFlashlight : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Flashlight Object")] private Light _flashlightObject;
    [SerializeField, Label("Flashlight Intensity")] private float _intensity = 1f;

    [Space, SerializeField, Label("Flashlight ON Sound Object")] private AudioSource _flashlightsSoundObjectOn;
    [SerializeField, Label("Flashlight OFF Sound Object")] private AudioSource _flashlightsSoundObjectOff;

    //Внутренние переменные


    //Кэшированные переменные


    //Методы Моно


    //Методы скрипта
    public void Toggle(string state = "~")
    {
        var initialIntensity = _flashlightObject.intensity;

        if (state == "~")
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
        }
        else if (state == "+")
        {
            initialIntensity = _intensity;
            _flashlightsSoundObjectOn.Play();
        }
        else
        {
            initialIntensity = 0f;
            _flashlightsSoundObjectOff.Play();
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