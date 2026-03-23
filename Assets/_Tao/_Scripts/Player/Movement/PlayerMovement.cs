using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStance))]
public class PlayerMovement : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Ground Checker Position")] private Transform _groundCheck;
    [SerializeField, Label("Ground Checker Length")] private float _groundCheckDistance = 1f;

    [Space, SerializeField, Label("Jump Height")] private float _jumpHeight = 1f;
    [SerializeField, Label("Jump Sound")] private AudioSource _jumpSoundObject;
    [SerializeField, Label("Grounded Sound")] private AudioSource _groundedSoundObject;

    [Space, SerializeField, Label("Steps Sound Object")] private AudioSource _stepsSoundObject;
    [SerializeField, Label("Steps Standart Sound")] private SoundMaterial _stepsStandartSound;
    [SerializeField, MinMaxSlider(-3f, 3f), Label("Steps Pitch Range")] private Vector2 _stepsPitchRange;

    [Space, SerializeField, Label("Gravity Force")] private float _gravityForce = 30f;

    //Внутренние переменные
    private float _currentSpeed;
    private bool _isGrounded = true;
    private RaycastHit _playerGroundHit;
    private float _gravitySpeed = 0f;

    //Кэшированные переменные
    private CharacterController _cc;
    private PlayerStance _playerStance;

    //Методы Моно
    private void Start()
    {
        _cc = GetComponent<CharacterController>();
        _playerStance = GetComponent<PlayerStance>();

        StanceUpdate();
    }

    private void Update()
    {
        GroundRayHit();
        StanceUpdate();
    }

    private void FixedUpdate()
    {
        GravityUpdate();
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

        Physics.Raycast(ray, out RaycastHit hit, _groundCheckDistance);

        if (hit.collider && hit.collider.gameObject.TryGetComponent<SurfaceMaterialHolder>(out var surfaceMaterialHolder))
        {
            var _stepSoundsAmount = surfaceMaterialHolder.MaterialSound.StepSounds.Count - 1;
            _stepsSoundObject.clip = surfaceMaterialHolder.MaterialSound.StepSounds[Random.Range(0, _stepSoundsAmount)];
        }
        else
        {
            var _stepSoundsAmount = _stepsStandartSound.StepSounds.Count - 1;
            _stepsSoundObject.clip = _stepsStandartSound.StepSounds[Random.Range(0, _stepSoundsAmount)];
        }

        if (!IsInvoking("PlaySound")) Invoke("PlaySound", 1 / _playerStance.StanceSpeed(_playerStance.CurrentStance) * 2);
    }

    private void GroundRayHit()
    {
        var ray = new Ray(_groundCheck.position, -transform.up);
        GroundSet(Physics.Raycast(ray, out _playerGroundHit, _groundCheckDistance));
    }

    private void GravityUpdate()
    {
        if (!_isGrounded) _gravitySpeed += _gravityForce * -9.81f * Time.fixedDeltaTime;
        _cc.Move(new Vector3(0, _gravitySpeed, 0));
    }

    private void PlaySound()
    {
        _stepsSoundObject.pitch = Random.Range(_stepsPitchRange.x, _stepsPitchRange.y);
        _stepsSoundObject.Play();
    }

    private void GroundSet(bool isGrounded)
    {
        if (_isGrounded == false && isGrounded == true)
        {
            Grounded();
        }
        else if (_isGrounded == true && isGrounded == false)
        {
            Ungrounded();
        }
    }

    private void Ungrounded()
    {
        _isGrounded = false;
    }

    private void Grounded()
    {
        _isGrounded = true;
        _gravitySpeed = 0f;
        _groundedSoundObject.Play();
    }

    public void Move(Vector2 direction)
    {
        var tripleAxisDirection = new Vector3(direction.x, 0, direction.y);
        var nextPosition = tripleAxisDirection * _currentSpeed * Time.fixedDeltaTime;

        _cc.Move(nextPosition);  

        if (direction.magnitude != 0f && _isGrounded) MakeStepSound();
        else CancelInvoke("PlaySound");
    }

    public void Jump(float strength)
    {
        if (_isGrounded && _playerStance.CurrentStance != PlayerStance.Stance.Crouching)
        {
            _gravitySpeed = strength;
            _jumpSoundObject.Play();
        }
    }

    //Геттеры и сеттеры
    public bool IsGrounded => _isGrounded;

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

    public AudioSource GroundedSoundObject
    {
        get => _groundedSoundObject;
        set => _groundedSoundObject = value;
    }

    public AudioSource JumpSoundObject
    {
        get => _jumpSoundObject;
        set => _jumpSoundObject = value;
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