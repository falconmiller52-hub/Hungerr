using UnityEngine;
using NaughtyAttributes;

public class PlayerScope : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Player Camera")] private Camera _playerCamera;

    [Space, SerializeField, Label("Raycasting Length")] private float _rayLength;


    //Внутренние переменные
    private RaycastHit _rayHit;
    private GameObject _interactableObject;

    //Кэшированные переменные


    //Методы Моно
    private void Update()
    {
        var ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(ray, out _rayHit, _rayLength);
        if (_rayHit.collider && _rayHit.collider.gameObject != _interactableObject && _rayHit.collider.gameObject.TryGetComponent<ObjectInteraction>(out ObjectInteraction _oi)) _interactableObject = _rayHit.collider.gameObject;
        else if (!_rayHit.collider) _interactableObject = null;
    }

    //Методы скрипта
    public void Interact()
    {
        _interactableObject?.GetComponent<ObjectInteraction>().InteractionEvents.Invoke();
    }

    //Геттеры и сеттеры


}