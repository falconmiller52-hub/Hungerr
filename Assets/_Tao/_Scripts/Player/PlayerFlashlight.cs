using UnityEngine;
using NaughtyAttributes;

public class PlayerFlashlight : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Camera Pivot Object")] private GameObject _cameraPivotObject;

    [Space, SerializeField, Label("Flashlight Object")] private GameObject _flashlightObject;
    [SerializeField, Label("Flashlight Smoothness")] private float _flashlightDamp = 1f;

    //Внутренние переменные


    //Кэшированные переменные


    //Методы Моно
    private void Start()
    {
        
    }

    private void Update()
    {
        var cameraRotation = _cameraPivotObject.transform.eulerAngles;
        _flashlightObject.transform.eulerAngles = cameraRotation;
       
    }


    //Методы скрипта


    //Геттеры и сеттеры
}