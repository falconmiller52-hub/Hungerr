using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class PlayerScope : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Camera Object")] private Transform _cameraObject;
    [SerializeField, Label("Scope Position Prefab")] private Transform _scopePositionPrefab;
    [SerializeField, Label("Indicatior Position Prefab")] private Transform _indicatorPositionPrefab;
    [SerializeField, Label("Scope Radius")] private float _scopeRadius = 1.25f;

    [Space, SerializeField, Label("Raycasting Length")] private float _rayLength;

    [Space, SerializeField, Label("Indicator Damp")] private float _indicatorDamp = 15f;

    //Внутренние переменные
    private RaycastHit _rayHit;
    private Transform _scopeObject, _indicatorObject;
    private bool _raycastResult;
    private GameObject _interactableObject;


    //Кэшированные переменные


    //Методы Моно
    private void Start()
    {
        _scopeObject = Instantiate(_scopePositionPrefab).transform;
        _indicatorObject = Instantiate(_indicatorPositionPrefab).transform;
    }

    private void Update()
    {
        var ray = new Ray(_cameraObject.position, _cameraObject.forward);
        _raycastResult = Physics.Raycast(ray, out _rayHit, _rayLength);

        _scopeObject.position = _raycastResult ? _rayHit.point : _cameraObject.position + _cameraObject.forward * _rayLength;
        
        var collisions = Physics.OverlapCapsule(_cameraObject.position, _scopeObject.position, _scopeRadius);
        var collision = collisions.FirstOrDefault(obj => obj.TryGetComponent<InteractionHolder>(out InteractionHolder interactionHolder));
        if (collision) _interactableObject = collision.gameObject;
        else _interactableObject = null;

        var nextPosition = Vector3.Lerp(_indicatorObject.position, collision ? _interactableObject.transform.position : _scopeObject.position, Time.deltaTime * _indicatorDamp);
        _indicatorObject.position = nextPosition;
    }


    //Методы скрипта


    //Геттеры и сеттеры
}