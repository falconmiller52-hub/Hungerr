using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStance))]
public class PlayerMovement : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Ground Checker Position")] private Transform _groundCheck;
    [SerializeField, Label("Ground Checker Length")] private float _groundCheckDistance = 1f;

    [Space, SerializeField, Label("Jump Height")] private float _jumpHeight = 1f;

    [Space, SerializeField, Label("Steps Sound Object")] private AudioSource _stepsSoundObject;
    [SerializeField, Label("Standart Sound")] private SoundMaterial _stepsStandartSound;

    //Внутренние переменные
    private float _currentSpeed;
    private bool _isGrounded = true;

    //Кэшированные переменные
    private Rigidbody _rigidbody;
    private PlayerStance _playerStance;

    //Методы Моно
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerStance = GetComponent<PlayerStance>();

        StanceUpdate();
    }

    private void Update()
    {
        _isGrounded = IsGrounded;

        StanceUpdate();
    }

    //Методы скрипта
    private void StanceUpdate()
    {
        var playerCurrentStance = _playerStance.CurrentStance;
        _currentSpeed = _playerStance.StanceSpeed(playerCurrentStance);
    }

    private void MakeStepSound()
    {
        var ray = new Ray(_groundCheck.position, -transform.up);

        RaycastHit hit;
        Physics.Raycast(ray, out hit, _groundCheckDistance);

        SurfaceMaterialHolder _surfaceMaterialHolder;
        if (hit.collider && hit.collider.gameObject.TryGetComponent<SurfaceMaterialHolder>(out _surfaceMaterialHolder))
        {
            var _stepSoundsAmount = _surfaceMaterialHolder.MaterialSound.StepSounds.Count - 1;
            _stepsSoundObject.clip = _surfaceMaterialHolder.MaterialSound.StepSounds[Random.Range(0, _stepSoundsAmount)];
        }
        else
        {
            var _stepSoundsAmount = _stepsStandartSound.StepSounds.Count - 1;
            _stepsSoundObject.clip = _stepsStandartSound.StepSounds[Random.Range(0, _stepSoundsAmount)];
        }

        if (!IsInvoking("PlaySound")) Invoke("PlaySound", 1 / _playerStance.StanceSpeed(_playerStance.CurrentStance) * 2);
    }

    private void PlaySound()
    {
        _stepsSoundObject.Play();
    }

    public void Move(Vector2 direction)
    {
        var tripleAxisDirection = new Vector3(direction.x, 0, direction.y);
        var nextPosition = transform.localPosition + tripleAxisDirection * _currentSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(nextPosition);

        if (direction.magnitude != 0f && _isGrounded) MakeStepSound();
        else CancelInvoke("PlaySound");
    }

    public void Jump(float strength)
    {
        var velocityChange = new Vector3(_rigidbody.velocity.x, strength, _rigidbody.velocity.z);
        if (_isGrounded && _playerStance.CurrentStance != PlayerStance.Stance.Crouching) _rigidbody.velocity = velocityChange;
    }

    //Геттеры и сеттеры
    public bool IsGrounded => Physics.Raycast(_groundCheck.position, -transform.up, _groundCheckDistance);

    public float JumpHeight
    {
        get => _jumpHeight;
        set => _jumpHeight = value;
    }

    public float CurrentSpeed
    {
        get => _currentSpeed;
        set => _currentSpeed = value;
    }

    public AudioSource StepsSoundObject
    {
        get => _stepsSoundObject;
        set => _stepsSoundObject = value;
    }

    public SoundMaterial StandartStepsSound
    {
        get => _stepsStandartSound;
        set => _stepsStandartSound = value;
    }
}