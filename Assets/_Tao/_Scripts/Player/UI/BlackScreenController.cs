using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class BlackScreenController : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Black Screen Object")] private Image _blackScreenObject;

    //Внутренние переменные


    //Кэшированные переменные


    //Методы Моно


    //Методы скрипта
    public void ChangeTransparency(float value)
    {
        _blackScreenObject.color = new Color(0f, 0f, 0f, value);
    }

    //Геттеры и сеттеры
    public Image BlackScreenObject
    {
        get => _blackScreenObject;
        set => _blackScreenObject = value;
    }
}