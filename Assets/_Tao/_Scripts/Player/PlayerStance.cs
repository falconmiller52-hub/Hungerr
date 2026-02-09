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
            if (_currentStamina < 100f)
            {
                if (_staminaWaiterTimer > 0f) _staminaWaiterTimer -= Time.deltaTime;
                else if (_staminaWaiterTimer <= 0f)
                {
                    var movementCoefficient = _playerMovement.MovingDirection.magnitude == 0f ? 1f : 0.8f;
                    _currentStamina += _staminaRegen * _staminaMultiplier * Time.deltaTime * movementCoefficient;
                }
            }
            if (_isExhausted)
            {
                if (_exhaustionTimer > 0f) _exhaustionTimer -= Time.deltaTime;
                else if (_exhaustionTimer <= 0f)
                {
                    _exhaustionTimer = _exhaustionDur;
                    _isExhausted = false;
                }
            }  
        }
    }

    public float CurrentStanceSpeed(Stance stance)
    {
        if (stance != Stance.Crouching && !_isExhausted)
        {
            if (stance == Stance.Running) return _stancesSpeeds.z;
            else return _stancesSpeeds.y;
        }
        else return _stancesSpeeds.x;
    }

    //Геттеры и сеттеры
    public Stance CurrentStance
    {
        get => _currentStance;
        set => _currentStance = value;
    }

    public Vector3 StancesSpeeds
    {
        get => _stancesSpeeds;
        set => _stancesSpeeds = value;
    }

    public float CurrentStamina
    {
        get => _currentStamina;
        set => _currentStamina = value;
    }

    public float MaximumStamina 
    {
        get => _maxStamina;
        set => _maxStamina = value;
    }

    public float StaminaUsage
    {
        get => _staminaUsage;
        set => _staminaUsage = value;
    }

    public float StaminaRegeneration
    {
        get => _staminaRegen;
        set => _staminaRegen = value;
    }

    public float ExhaustionDuration
    {
        get => _exhaustionDur;
        set => _exhaustionDur = value;
    }

    public float ExhaustionTimer
    {
        get => _exhaustionTimer;
        set => _exhaustionTimer = value;
    }

    public float StaminaWaiter
    {
        get => _staminaWaiter;
        set => _staminaWaiter = value;
    }

    public float StaminaWaiterTimer
    {
        get => _staminaWaiterTimer;
        set => _staminaWaiterTimer = value;
    }

    public float StaminaMultiplier
    {
        get => _staminaMultiplier;
        set => _staminaMultiplier = value;
    }
}