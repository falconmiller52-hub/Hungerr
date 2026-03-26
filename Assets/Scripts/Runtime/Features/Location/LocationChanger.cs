using System.Collections;
using NaughtyAttributes;
using Runtime.Features.Player.UI;
using UnityEngine;

namespace Runtime.Features.Location
{
    public class LocationChanger : MonoBehaviour
    {
        //Переменные инспектора
        [SerializeField, Label("Next Position Object")] private Transform _nextPositionObject;

        [Space, SerializeField, Label("Transition Speed")] private Vector2 _transitionSpeed = Vector2.right;

        [Space, SerializeField, Label("Start Sound")] private AudioClip _startSound;
        [SerializeField, Label("End Sound")] private AudioClip _endSound;

        //Внутренние переменные
        private static bool _isCoroutineActive = false;

        //Кэшированные переменные
        private Transform _theFuckingPlayerItself;
        private AudioSource _as;
        private BlackScreenController _bsc;
        private CharacterController _cc;

        //Методы Моно
        private void Start()
        {
            _theFuckingPlayerItself = FindObjectOfType<CharacterController>().transform;
            _as = GetComponent<AudioSource>();
            _bsc = _theFuckingPlayerItself.GetComponent<BlackScreenController>();
            _cc = _theFuckingPlayerItself.GetComponent<CharacterController>();
        }

        //Методы скрипта
        public void ChangeLocation()
        {
            if (!_isCoroutineActive) StartCoroutine(LocationChangeAnimation());
        }

        private IEnumerator LocationChangeAnimation()
        {
            _isCoroutineActive = true;
            _as.PlayOneShot(_startSound);
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

            _as.PlayOneShot(_endSound);
            for (float i = _bsc.BlackScreenObject.color.a; i >= 0f; i -= Time.deltaTime)
            {
                _bsc.BlackScreenObject.color = Color.black * i;
                yield return new WaitForSeconds(_transitionSpeed.x);
            }

            _bsc.BlackScreenObject.color = Color.black * 0f;

            _isCoroutineActive = false;
            yield return null;
        }

        //Геттеры и сеттеры
        public Transform NextPositionObject
        {
            get => _nextPositionObject;
            set => _nextPositionObject = value;
        }
    }
}