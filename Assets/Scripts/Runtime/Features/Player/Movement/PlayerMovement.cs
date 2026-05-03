using NaughtyAttributes;
using Runtime.Common.Services.Audio.Sound;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.Pause;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Movement
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(PlayerStance))]
	[RequireComponent(typeof(PlayerMovementStepSound))]
	public class PlayerMovement : MonoBehaviour, IPausable
	{
		//Переменные инспектора
		[SerializeField, Label("Ground Checker Position")]
		private Transform _groundCheck;

		[SerializeField, Label("Ground Checker Length")]
		private float _groundCheckDistance = 1f;

		[Space, SerializeField, Label("Gravity Force")]
		private float _gravityForce = 30f;

		//Внутренние переменные
		private float _currentSpeed;
		private bool _isGrounded = true;
		private RaycastHit _playerGroundHit;
		private float _gravitySpeed = 0f;
		private Vector2 _inputDirection;
		private bool _isCanMove = true;

		//Кэшированные переменные
		private Camera _playerCamera;
		private CharacterController _cc;
		private PlayerStance _playerStance;
		private PlayerMovementStepSound _playerMovementStepSound;
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
			_playerMovementStepSound = GetComponent<PlayerMovementStepSound>();
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
		}

		public void Move(Vector2 direction)
		{
			var tripleAxisDirection = new Vector3(direction.x, 0, direction.y);
			var nextPosition = tripleAxisDirection * _currentSpeed * Time.deltaTime;

			_cc.Move(nextPosition);

			if (direction.magnitude != 0f && _isGrounded)
				_playerMovementStepSound.StartMoveSound();
			else
				_playerMovementStepSound.StopMoveSound();
		}


		private void SetNewMoveInputDirection(Vector2 inputDirection)
		{
			_inputDirection = inputDirection;
		}

		private Vector3 GetPlayerCameraForward()
		{
			Vector3 forward = _playerCamera.transform.forward;
			forward.y = 0;
			return forward.normalized;
		}

		private Vector3 GetPlayerCameraRight()
		{
			Vector3 right = _playerCamera.transform.right;
			right.y = 0;
			return right.normalized;
		}

		//Геттеры и сеттеры

		public Vector2 MovingDirection
		{
			get
			{
				var moveDirection = (GetPlayerCameraForward() * _inputDirection.y)
				                    + (GetPlayerCameraRight() * _inputDirection.x);
				var directionResult = new Vector2(moveDirection.x, moveDirection.z);
				return directionResult.normalized;
			}
		}

		public Transform GroundCheck => _groundCheck;

		public float GroundCheckDistance => _groundCheckDistance;
	}
}