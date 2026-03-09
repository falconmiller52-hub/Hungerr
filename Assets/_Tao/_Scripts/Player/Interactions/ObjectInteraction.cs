using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class ObjectInteraction : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Interaction Events")] private UnityEvent _interactionEvents;

    //Внутренние переменные


    //Кэшированные переменные


    //Методы Моно


    //Методы скрипта


    //Геттеры и сеттеры
    public UnityEvent InteractionEvents
    {
        get => _interactionEvents;
        set => _interactionEvents = value;
    }
}