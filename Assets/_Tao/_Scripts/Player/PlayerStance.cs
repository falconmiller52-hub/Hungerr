using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerStance : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Camera Pivot")] Transform _playerCamera; 
    [SerializeField, Label("Player Model")] GameObject _playerModel; 

    [Space, SerializeField, Label("Speeds of Stances")] Vector3 _stancesSpeeds = Vector3.one;

    [Space, SerializeField, Label("Is player running?")] bool _isRunning = false;
    [SerializeField, Label("Is player crouching?")] bool _isCrouching = false;

    [Space, SerializeField, Label("Player's Max Stamina")] float _maxStamina = 100f;
    [SerializeField, Label("Stamina Consumption")] float _staminaUse = 1f;
    [SerializeField, Label("Stamina Regenetration")] float _staminaHeal = 1f;
    [SerializeField, Label("Stamina Change Multiplier")] float _staminaMultiplier = 1f;

    [SerializeField, Label("Crouch Smoothness")] float _crouchSmooth = 1f;

    //Внутренние переменные
    float _currentStamina;

    //Кэширование
    PlayerMovement _playerMovement;
    CapsuleCollider _capsuleCollider;

    //Инициализация
    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _capsuleCollider = GetComponentInChildren<CapsuleCollider>();

        _currentStamina = _maxStamina;
    }

    //Постоянное обновление движка
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) _isCrouching = !_isCrouching;

        if (Input.GetKey(KeyCode.LeftShift)) _isRunning = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift)) _isRunning = false;

        StaminaManagement();
        ChangeSize();

        Run(_isRunning);
        Crouch(_isCrouching);
    }

    //Стамина
    private void StaminaManagement()
    {
        _currentStamina -= _isRunning && !_isCrouching && _playerMovement.PlayerMovingDirection.magnitude != 0f ? _staminaUse * Time.deltaTime * _staminaMultiplier : -_staminaHeal * Time.deltaTime * _staminaMultiplier;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
    }

    //Изменение размеров игрока
    private void ChangeSize()
    {
        var colliderNewSize = _isCrouching ? 1f : 2f;
        var pivotNewPosition = _isCrouching ? Vector3.up * 0.32f : Vector3.up * 0.79f;

        _capsuleCollider.height = Mathf.Lerp(_capsuleCollider.height, colliderNewSize, Time.deltaTime * _crouchSmooth);
        _playerCamera.localPosition = Vector3.Lerp(_playerCamera.localPosition, pivotNewPosition, Time.deltaTime * _crouchSmooth);
        _playerModel.SetActive(_playerCamera.localPosition.magnitude >= 0.72f);
    }

    //Бег
    public void Run(bool isRunning)
    {
        if (isRunning && !_isCrouching && _currentStamina > 0f) _playerMovement.CurrentSpeed = _stancesSpeeds.z;
        else if (!isRunning || _currentStamina == 0f) _playerMovement.CurrentSpeed = _stancesSpeeds.y;
    }

    //Присяд
    public void Crouch(bool isCrouching)
    {
        if (_isRunning && isCrouching) _isRunning = false;

        if (isCrouching) _playerMovement.CurrentSpeed = _stancesSpeeds.x;
        else if (!isCrouching && !_isRunning) _playerMovement.CurrentSpeed = _stancesSpeeds.y;
    }

    //Геттеры и сеттеры
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

    public float StaminaUse
    {
        get => _staminaUse;
        set => _staminaUse = value;
    }

    public float StaminaMultiplier
    {
        get => _staminaMultiplier;
        set => _staminaMultiplier = value;
    }

    public float StaminaRegeneration
    {
        get => _staminaHeal;
        set => _staminaHeal = value;
    }

    public float MaxStamina
    {
        get => _maxStamina;
        set => _maxStamina = value;
    }

    public float CrouchSmoothness
    {
        get => _crouchSmooth;
        set => _crouchSmooth = value;
    }

    public bool IsRunning => _isRunning;
    public bool IsCrouching => _isCrouching;
}
