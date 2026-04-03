using NaughtyAttributes;
using Runtime.Common.Services.Audio;
using Runtime.Common.Services.Input;
using Runtime.Features.Sounds;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerStance))]
    public class PlayerMovement : MonoBehaviour
    {
        //Переменные инспектора
        [SerializeField, Label("Ground Checker Position")] private Transform _groundCheck;
        [SerializeField, Label("Ground Checker Length")] private float _groundCheckDistance = 1f;

        [Space, SerializeField, Label("Jump Height")] private float _jumpHeight = 1f;
        
        [SerializeField, Label("Standard Step Sound")] private SoundData _standartStepSound;
        [SerializeField, Tooltip("Ground Sound On Start Jump")] private SoundData _jumpStartSound;
        [SerializeField, Tooltip("Ground Sound After Jump")] private SoundData _jumpEndSound;

        [Space, SerializeField, Label("Gravity Force")] private float _gravityForce = 30f;

        //Внутренние переменные
        private float _currentSpeed;
        private bool _isJumpInputActive;
        private bool _isGrounded = true;
        private RaycastHit _playerGroundHit;
        private float _gravitySpeed = 0f;
        private Vector2 _inputDirection;
        private SoundData _currentStepSoundData;

        //Кэшированные переменные
        private CharacterController _cc;
        private PlayerStance _playerStance;
        private IInputHandler _inputHandler;
        private IAudioService _audioService;

        [Inject]
        private void Construct(IInputHandler inputHandler, IAudioService audioService)
        {
            _inputHandler = inputHandler;
            _audioService = audioService;
        }
    
        private void Start()
        {
            _cc = GetComponent<CharacterController>();
            _playerStance = GetComponent<PlayerStance>();

            StanceUpdate();
        }
    
        private void OnEnable()
        {
            if (_inputHandler == null)
            {
                Debug.LogError("PlayerMovement::OnEnable() No Input Handler was assigned");
                return;
            }
        
            _inputHandler.PlayerMoveInputChanged += SetNewMoveInputDirection;
            _inputHandler.JumpInputPressed += SetJumpInputPressed;
        }

        private void OnDisable()
        {
            if (_inputHandler == null)
            {
                Debug.LogError("PlayerMovement::OnDisable() No Input Handler was assigned");
                return;
            }
        
            _inputHandler.PlayerMoveInputChanged -= SetNewMoveInputDirection;
            _inputHandler.JumpInputPressed -= SetJumpInputPressed;
        }

        private void Update()
        {
            GroundRayHit();
            StanceUpdate();
            Move(MovingDirection);
            
            if (_isJumpInputActive)
                Jump(_jumpHeight);
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

            if (hit.collider && hit.collider.gameObject.TryGetComponent<SurfaceMaterialSoundHolder>(out var surfaceMaterialHolder))
            {
                var stepSoundsAmount = surfaceMaterialHolder.MaterialSounds.Count;
                _currentStepSoundData = surfaceMaterialHolder.MaterialSounds[Random.Range(0, stepSoundsAmount)];
            }
            else
            {
                _currentStepSoundData = _standartStepSound;
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
            _audioService.PlaySfx(_currentStepSoundData, transform.position);
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
            _audioService.PlaySfx(_jumpEndSound, _groundCheck.position);
        }

        public void Move(Vector2 direction)
        {
            var tripleAxisDirection = new Vector3(direction.x, 0, direction.y);
            var nextPosition = tripleAxisDirection * _currentSpeed * Time.deltaTime;

            _cc.Move(nextPosition);  

            if (direction.magnitude != 0f && _isGrounded) MakeStepSound();
            else CancelInvoke("PlaySound");
        }

        public void Jump(float strength)
        {
            if (_isGrounded && _playerStance.CurrentStance != PlayerStance.Stance.Crouching)
            {
                _gravitySpeed = strength;
                _audioService.PlaySfx(_jumpStartSound, _groundCheck.position);
            }
        }

        private void SetNewMoveInputDirection(Vector2 inputDirection)
        {
            _inputDirection = inputDirection;
        }

        private void SetJumpInputPressed(bool isPressed)
        {
            _isJumpInputActive = isPressed;
        }

        //Геттеры и сеттеры
    
        public Vector2 MovingDirection
        {
            get
            {
                var moveDirection = transform.forward * _inputDirection.y + transform.right * _inputDirection.x;
                var directionResult = new Vector2(moveDirection.x, moveDirection.z);
                return directionResult;
            }
        }
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
    }
}