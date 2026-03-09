using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class LocationChanger : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Player")] private Transform _theFuckingPlayerItself;
    [SerializeField, Label("Next Position Object")] private Transform _nextPositionObject;

    [Space, SerializeField, Label("Transition Speed")] private Vector2 _transitionSpeed = Vector2.right;

    //Внутренние переменные


    //Кэшированные переменные
    BlackScreenController _bsc;
    CharacterController _cc;

    //Методы Моно
    private void Start()
    {
        _bsc = _theFuckingPlayerItself.GetComponent<BlackScreenController>();
        _cc = _theFuckingPlayerItself.GetComponent<CharacterController>();
    }

    //Методы скрипта
    public void ChangeLocation()
    {
        StartCoroutine(LocationChangeAnimation());
    }

    private IEnumerator LocationChangeAnimation()
    {
        for (float i = _bsc.BlackScreenObject.color.a; i <= 1f; i += Time.deltaTime)
        {
            _bsc.BlackScreenObject.color = Color.black * i;
            yield return new WaitForSeconds(_transitionSpeed.x);
        }

        _bsc.BlackScreenObject.color = Color.black * 1f;

        yield return new WaitForSeconds(_transitionSpeed.y);

        _cc.enabled = false;
        _theFuckingPlayerItself.position = _nextPositionObject.position;
        _cc.enabled = true;

        for (float i = _bsc.BlackScreenObject.color.a; i >= 0f; i -= Time.deltaTime)
        {
            _bsc.BlackScreenObject.color = Color.black * i;
            yield return new WaitForSeconds(_transitionSpeed.x);
        }

        _bsc.BlackScreenObject.color = Color.black * 0f;

        yield return null;
    }

    //Геттеры и сеттеры
    public Transform NextPositionObject
    {
        get => _nextPositionObject;
        set => _nextPositionObject = value;
    }
}