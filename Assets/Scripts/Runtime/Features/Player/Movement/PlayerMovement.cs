using FMODUnity;
using NaughtyAttributes;
using Runtime.Common.Extensions;
using Runtime.Common.Services.Audio;
using Runtime.Common.Services.Audio.Sound;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.Pause;
using Runtime.Features.Sounds;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Movement
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(PlayerStance))]
	public class PlayerMovement : MonoBehaviour, IPausable
	{
		//Переменные инспектора
		[SerializeField, Label("Ground Checker Position")]
		private Transform _groundCheck;

		[SerializeField, Label("Ground Checker Length")]
		private float _groundCheckDistance = 1f;

		[Space, SerializeField, Label("Jump Height")]
		private float _jumpHeight = 1f;

		[SerializeField, Label("Standard Step Sound")]
		private EventReference _standartStepSound;

		[SerializeField, Tooltip("Ground Sound On Start Jump")]
		private EventReference _jumpStartSound;

		[SerializeField, Tooltip("Ground Sound After Jump")]
		private EventReference _jumpEndSound;

		[Space, SerializeField, Label("Gravity Force")]
		private float _gravityForce = 30f;

		//Внутренние переменные
		private float _currentSpeed;
		private bool _isJumpInputActive;
		private bool _isGrounded = true;
		private RaycastHit _playerGroundHit;
		private float _gravitySpeed = 0f;
		private Vector2 _inputDirection;
		private EventReference _currentStepSoundData;
		private bool _isCanMove = true;

		//Кэшированные переменные
		private Camera _playerCamera;
		private CharacterController _cc;
		private PlayerStance _playerStance;
		private IInputHandler _inputHandler;
		private ISoundService _soundService;
		private IPauseController _pauseController;

		[Inject]
		private void Construct(IInputHandler inputHandler, ISoundService soundService, IPauseController pauseController)
		{
			_inputHandler = inputHandler;
			_soundService = soundService;
			_pauseController = pauseController;
		}

		private void Start()
		{
			_cc = GetComponent<CharacterController>();
			_playerStance = GetComponent<PlayerStance>();
			_playerCamera = Camera.main;
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

			if (_pauseController == null)
			{
				Debug.LogError("PlayerMovement::OnEnable() No Pause Controller was assigned");
				return;
			}

			_pauseController.Add(this);
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

			if (_pauseController == null)
			{
				Debug.LogError("PlayerMovement::OnDisable() No Pause Controller was assigned");
				return;
			}

			_pauseController.Remove(this);
		}

		private void Update()
		{
			// Сделал проверку тут, т.к когда isCanMove нельзя не только двигаться, но и прыгать, издавать звуки и т.д.
			if (_isCanMove)
			{
				GroundRayHit();
				StanceUpdate();
				Move(MovingDirection);

				if (_isJumpInputActive)
					Jump(_jumpHeight);
			}
		}

		private void FixedUpdate()
		{
			GravityUpdate();
		}

		//Методы скрипта
		public void Stop()
			=> SetDisableMove();

		public void Resume()
			=> SetEnableMove();

		private void SetEnableMove()
			=> _isCanMove = true;

		private void SetDisableMove()
			=> _isCanMove = false;

		private void StanceUpdate()
		{
			var playerCurrentStance = _playerStance.CurrentStance;
			_currentSpeed = _playerStance.StanceSpeed(playerCurrentStance);
		}

		private void MakeStepSound()
		{
			var ray = new Ray(_groundCheck.position, -transform.up);

			Physics.Raycast(ray, out RaycastHit hit, _groundCheckDistance);

			if (hit.collider &&
			    hit.collider.gameObject.TryGetComponent<SurfaceMaterialSoundHolder>(out var surfaceMaterialHolder))
			{
				_currentStepSoundData = surfaceMaterialHolder.MaterialSounds.Random();
			}
			else
			{
				_currentStepSoundData = _standartStepSound;
			}

			if (!IsInvoking("PlaySound"))
				Invoke("PlaySound", 1 / _playerStance.StanceSpeed(_playerStance.CurrentStance) * 2);
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
			_soundService.PlaySound(_currentStepSoundData, transform.position);
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
			_soundService.PlaySound(_jumpEndSound, _groundCheck.position);
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
				_soundService.PlaySound(_jumpStartSound, _groundCheck.position);
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
				var moveDirection = (_playerCamera.transform.forward * _inputDirection.y)
				                    + (_playerCamera.transform.right * _inputDirection.x);
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