using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class PlayerScope : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Player Camera")] private Camera _playerCamera;

    [Space, SerializeField, Label("Raycasting Length")] private float _rayLength;

    //Внутренние переменные
    private RaycastHit _rayHit;
    private GameObject _interactableObject;
    private ObjectInteraction _interaction;

    //Кэшированные переменные


    //Методы Моно
    private void Update()
    {
        var ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(ray, out _rayHit, _rayLength);

        if (_rayHit.collider && _rayHit.collider.gameObject != _interactableObject && _rayHit.collider.gameObject.TryGetComponent<ObjectInteraction>(out ObjectInteraction _oi))
        {
            _interactableObject = _rayHit.collider.gameObject;

            _interaction = _interactableObject.GetComponent<ObjectInteraction>();
            _interaction.CurrentOutlineWidth = 25f;
        }
        else if ((!_rayHit.collider || _rayHit.collider && _rayHit.collider.gameObject != _interactableObject) && _interactableObject) 
        {
            _interaction.CurrentOutlineWidth = 0f;

            _interaction = null;
            _interactableObject = null;
        } 
    }

    //Методы скрипта
    public void Interact()
    {
        if (_interaction) _interaction.InteractionEvents.Invoke();
    }

    //Геттеры и сеттеры


}