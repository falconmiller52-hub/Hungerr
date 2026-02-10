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

    [Space, SerializeField, Label("Crouching Speed")] private float _crouchingSpeed = 1f;
    [SerializeField, Label("Crouching Cooldown")] private float _crouchingCooldown = 1f;

    //Внутренние переменные
    public enum Stance
    {
        Walking,
        Running,
        Crouching
    }

    private bool _isExhausted = false;
    private float _currentStamina, _staminaWaiterTimer, _exhaustionTimer, _crouchingTimer;

    //Кэшированные переменные
    PlayerMovement _playerMovement;
    CapsuleCollider _capsuleCollider;

    //Методы Моно
    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _capsuleCollider = GetComponentInChildren<CapsuleCollider>();

        _currentStamina = _maxStamina;
        _staminaWaiterTimer = _staminaWaiter;
        _exhaustionTimer = _exhaustionDur;
        _crouchingTimer = _crouchingCooldown;
    }

    private void Update()
    {
        StanceChange();
        StaminaChange();
        CrouchChange();
    }

    //Методы скрипта
    private void StanceChange()
    {
        _crouchingTimer = Mathf.Clamp(_crouchingTimer, 0, _crouchingCooldown);
        if (_crouchingTimer < _crouchingCooldown) _crouchingTimer += Time.deltaTime;

        var crouchCondition = Input.GetKeyDown(KeyCode.C) && _crouchingTimer >= _crouchingCooldown;

        if (_currentStance == Stance.Running)
        {
            if (crouchCondition)
            {
                _currentStance = Stance.Crouching;
                _crouchingTimer = 0f;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) || _currentStamina <= 0f)
            {
                _currentStance = Stance.Walking;
            }
        }
        else if (_currentStance == Stance.Crouching)
        {
            if (crouchCondition)
            {
                _currentStance = Stance.Walking;
                _crouchingTimer = 0f;
            }
        }
        else
        {
            if (crouchCondition)
            {
                _currentStance = Stance.Crouching;
                _crouchingTimer = 0f;
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

    private void CrouchChange()
    {
        _capsuleCollider.height = Mathf.Lerp(_capsuleCollider.height, _currentStance == Stance.Crouching ? 1f : 2f, Time.deltaTime * _crouchingSpeed);
        _capsuleCollider.center = Vector3.Lerp(_capsuleCollider.center, _currentStance == Stance.Crouching ? Vector3.up * -0.5f : Vector3.zero, Time.deltaTime * _crouchingSpeed);
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