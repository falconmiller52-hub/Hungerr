using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerStance : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Current Player Stance")] private Stance _currentStance;
    [SerializeField, Label("Stances Speeds")] private Vector3 _stancesSpeeds = Vector3.one;

    [Space, SerializeField, Label("Maximum Stamina")] private float _maxStamina = 100f;
    [SerializeField, Label("Stamina Usage")] private float _staminaUsage = 15f;
    [SerializeField, Label("Stamina Regeneration")] private float _staminaRegen = 20f;
    [SerializeField, Label("Exhaustion Duration")] private float _exhaustionDur = 7f;
    [SerializeField, Label("Stamina Wait To Regen")] private float _staminaWaiter = 3f;
    [SerializeField, Label("Stamina Multiplier")] private float _staminaMultiplier = 1f;

    //Внутренние переменные
    public enum Stance
    {
        Walking,
        Running,
        Crouching
    }

    private bool _isExhausted = false;
    private float _currentStamina, _staminaWaiterTimer, _exhaustionTimer;

    //Кэшированные переменные
    PlayerMovement _playerMovement;

    //Методы Моно
    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();

        _currentStamina = _maxStamina;
        _staminaWaiterTimer = _staminaWaiter;
        _exhaustionTimer = _exhaustionDur;
    }

    private void Update()
    {
        ChangeStance();
        StaminaChange();
    }

    //Методы скрипта
    private void ChangeStance()
    {
        if (_currentStance == Stance.Running)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                _currentStance = Stance.Crouching;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) || _currentStamina <= 0f)
            {
                _currentStance = Stance.Walking;
            }
        }
        else if (_currentStance == Stance.Crouching)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                _currentStance = Stance.Walking;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                _currentStance = Stance.Crouching;
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift) && !_isExhausted)
            {
                _currentStance = Stance.Running;
            }
        }
    }

    private void StaminaChange()
    {
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
        _staminaWaiterTimer = Mathf.Clamp(_staminaWaiterTimer, 0, _staminaWaiter);
        _exhaustionTimer = Mathf.Clamp(_exhaustionTimer, 0, _exhaustionDur);

        if (_currentStance == Stance.Running && _playerMovement.MovingDirection.magnitude > 0f && !_isExhausted)
        {
            _staminaWaiterTimer = _staminaWaiter;
            _currentStamina -= _staminaUsage * _staminaMultiplier * Time.deltaTime;

            if (_currentStamina <= 0f) _isExhausted = true;
        }
        else
        {
            if (_staminaWaiterTimer > 0f && _currentStamina < 100f) _staminaWaiterTimer -= Time.deltaTime;
            else if (_staminaWaiterTimer <= 0f && _currentStamina < 100f)
            {
                var movementCoefficient = _playerMovement.MovingDirection.magnitude == 0f ? 1f : 0.8f;
                _currentStamina += _staminaRegen * _staminaMultiplier * Time.deltaTime * movementCoefficient;
            }

            if (_exhaustionTimer > 0f && _isExhausted) _exhaustionTimer -= Time.deltaTime;
            else if (_exhaustionTimer <= 0f && _isExhausted) _isExhausted = false;
        }
    }

    public float CurrentStanceSpeed(Stance stance)
    {
        var speed = 0f;
        if (stance == Stance.Running && !_isExhausted) speed = _stancesSpeeds.z;
        else if (stance == Stance.Crouching || _isExhausted) speed = _stancesSpeeds.x;
        else speed = _stancesSpeeds.y;
        return speed;
    }

    //Геттеры и сеттеры

}