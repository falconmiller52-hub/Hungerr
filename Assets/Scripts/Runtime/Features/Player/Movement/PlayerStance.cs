using System.Collections;
using FMODUnity;
using NaughtyAttributes;
using Runtime.Common.Enums;
using Runtime.Common.Services.Audio;
using Runtime.Common.Services.EventBus;
using Runtime.Common.Services.Input;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Movement
{
	public class PlayerStance : MonoBehaviour
	{
		[Header("Components")] [SerializeField]
		private Transform _playerTransform;

		[SerializeField] [Label("GameObject который регулирует позицию камеры")]
		private GameObject _playerPivotParentObj;

		//Переменные инспектора
		[Header("Settings")] [SerializeField, Label("Current Player Stance")]
		private Stance _currentStance;

		[SerializeField, Label("Stances Speeds")]
		private Vector3 _stancesSpeeds = Vector3.one;

		[Space, SerializeField, Label("Maximum Stamina")]
		private float _maxStamina = 100f;

		[SerializeField, Label("Stamina Usage")]
		private float _staminaUsage = 15f;

		[SerializeField, Label("Stamina Regeneration")]
		private float _staminaRegen = 20f;

		[SerializeField, Label("Exhaustion Duration")]
		private float _exhaustionDur = 7f;

		[SerializeField, Label("Stamina Wait To Regen")]
		private float _staminaWaiter = 3f;

		[SerializeField, Label("Stamina Multiplier")]
		private float _staminaMultiplier = 1f;

		[Space, SerializeField, Label("Crouching Speed")]
		private float _crouchingSpeed = 1f;

		[SerializeField, Label("Crouching Cooldown")]
		private float _crouchingCooldown = 1f;

		[SerializeField, Label("Crouching Volumes")]
		private Vector2 _crouchingVolumes = Vector2.one;

		[Space, SerializeField, Label("Ceiling Checker Position")]
		private Transform _ceilingCheck;

		[SerializeField, Label("Ceiling Checker Length")]
		private float _ceilingCheckDistance = 1f;

		[SerializeField, Label("Exhaustion Sound")]
		private EventReference _exhaustionStepSound;

		//Внутренние переменные
		public enum Stance
		{
			Walking,
			Running,
			Crouching
		}

		private bool _isExhausted = false, _isUnderCeiling = false;
		private float _currentStamina, _staminaWaiterTimer, _exhaustionTimer, _crouchingTimer;
		private bool _runPress = false, _crouchPress = false;
		private Vector2 _inputDirection;

		//Кэшированные переменные
		private IInputHandler _inputHandler;
		private IAudioService _audioService;
		private CharacterController _cc;
		private EventBus _eventBus;

		[Inject]
		private void Construct(IInputHandler inputHandler, IAudioService audioService, EventBus eventBus)
		{
			_inputHandler = inputHandler;
			_audioService = audioService;
			_eventBus = eventBus;
		}

		private void Start()
		{
			_cc = GetComponentInChildren<CharacterController>();

			_currentStamina = _maxStamina;
			_staminaWaiterTimer = _staminaWaiter;
			_exhaustionTimer = _exhaustionDur;
			_crouchingTimer = _crouchingCooldown;
		}

		private void OnEnable()
		{
			if (_inputHandler == null)
			{
				Debug.LogError("PlayerStance::OnEnable() No Input Handler was assigned");
				return;
			}

			_inputHandler.PlayerMoveInputChanged += SetNewMoveInputDirection;
			_inputHandler.RunInputPressed += Run;
			_inputHandler.CrouchInputPressed += Crouch;
		}

		private void OnDisable()
		{
			if (_inputHandler == null)
			{
				Debug.LogError("PlayerStance::OnDisable() No Input Handler was assigned");
				return;
			}

			_inputHandler.PlayerMoveInputChanged -= SetNewMoveInputDirection;
			_inputHandler.RunInputPressed -= Run;
			_inputHandler.CrouchInputPressed -= Crouch;
		}

		private void Update()
		{
			_isUnderCeiling = IsUnderCeiling;

			StanceChange();
			StaminaChange();
			CrouchChange();
		}

		//Методы скрипта
		private void StanceChange()
		{
			_crouchingTimer = Mathf.Clamp(_crouchingTimer, 0, _crouchingCooldown);
			if (_crouchingTimer < _crouchingCooldown) _crouchingTimer += Time.deltaTime;

			var crouchCondition = _crouchingTimer >= _crouchingCooldown && !_isUnderCeiling;

			// Run state
			if (_currentStance == Stance.Running)
			{
				if (_crouchPress && crouchCondition)
				{
					_eventBus.Trigger(EPlayerStanceEvent.StartCrouchState);

					_currentStance = Stance.Crouching;
					_crouchingTimer = 0f;
				}
				else if (!_runPress || _currentStamina <= 0f)
				{
					_eventBus.Trigger(EPlayerStanceEvent.StartWalkState);

					_currentStance = Stance.Walking;
				}
			}
			// Crouch state
			else if (_currentStance == Stance.Crouching)
			{
				if (_crouchPress && crouchCondition)
				{
					_eventBus.Trigger(EPlayerStanceEvent.StartWalkState);

					_currentStance = Stance.Walking;
					_crouchingTimer = 0f;
				}
			}
			// Walk state
			else
			{
				if (_crouchPress && crouchCondition)
				{
					_eventBus.Trigger(EPlayerStanceEvent.StartCrouchState);

					_currentStance = Stance.Crouching;
					_crouchingTimer = 0f;
				}
				else if (_runPress && !_isExhausted)
				{
					_eventBus.Trigger(EPlayerStanceEvent.StartRunState);

					_currentStance = Stance.Running;
				}
			}
		}

		private void StaminaChange()
		{
			_currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
			_staminaWaiterTimer = Mathf.Clamp(_staminaWaiterTimer, 0, _staminaWaiter);
			_exhaustionTimer = Mathf.Clamp(_exhaustionTimer, 0, _exhaustionDur);

			if (_currentStance == Stance.Running && _playerTransform.forward.magnitude > 0f && !_isExhausted)
			{
				_staminaWaiterTimer = _staminaWaiter;
				_currentStamina -= _staminaUsage * _staminaMultiplier * Time.deltaTime;

				if (_currentStamina <= 0f)
				{
					_isExhausted = true;
					_audioService.PlaySfx(_exhaustionStepSound, transform.position);
				}
			}
			else
			{
				if (_currentStamina < 100f)
				{
					if (_staminaWaiterTimer > 0f) _staminaWaiterTimer -= Time.deltaTime;
					else if (_staminaWaiterTimer <= 0f)
					{
						var movementCoefficient = _playerTransform.forward.magnitude == 0f ? 1f : 0.8f;
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
			// Todo: ну тут пиздец, Я зарахкодил чтобы сейчас работало, но потом нужно менять
			_cc.height = Mathf.Lerp(_cc.height, _currentStance == Stance.Crouching ? 1f : 1.7f,
							Time.deltaTime * _crouchingSpeed);
			_cc.center = Vector3.Lerp(_cc.center,
							_currentStance == Stance.Crouching ? Vector3.up * -0.5f : Vector3.zero,
							Time.deltaTime * _crouchingSpeed);
			_playerPivotParentObj.transform.localPosition = Vector3.Lerp(_playerPivotParentObj.transform.localPosition,
							_currentStance == Stance.Crouching ? Vector3.zero : Vector3.up,
							Time.deltaTime * _crouchingSpeed);
		}

		public float StanceSpeed(Stance stance)
		{
			if (stance != Stance.Crouching && !_isExhausted)
			{
				if (stance == Stance.Running) return _stancesSpeeds.z;
				else return _stancesSpeeds.y;
			}
			else return _stancesSpeeds.x;
		}

		private IEnumerator DelayedCrouch()
		{
			_crouchPress = true;
			yield return new WaitForSeconds(Time.deltaTime);
			_crouchPress = false;
		}

		public void Crouch()
		{
			StartCoroutine("DelayedCrouch");
		}

		public void Run(bool pressed)
		{
			_runPress = pressed;
		}

		private void SetNewMoveInputDirection(Vector2 inputDirection)
		{
			_inputDirection = inputDirection;
		}

		//Геттеры и сеттеры
		public bool IsUnderCeiling => Physics.Raycast(_ceilingCheck.position, transform.up, _ceilingCheckDistance);

		public Vector2 MovingDirection
		{
			get
			{
				var moveDirection = transform.forward * _inputDirection.y + transform.right * _inputDirection.x;
				var directionResult = new Vector2(moveDirection.x, moveDirection.z);
				return directionResult;
			}
		}

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

		public bool IsExhausted => _isExhausted;

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

		public float CrouchSpeed
		{
			get => _crouchingSpeed;
			set => _crouchingSpeed = value;
		}

		public float CrouchCooldown
		{
			get => _crouchingCooldown;
			set => _crouchingCooldown = value;
		}

		public Vector2 CrouchingVolumes
		{
			get => _crouchingVolumes;
			set => _crouchingVolumes = value;
		}

		public float CrouchTimer
		{
			get => _crouchingTimer;
			set => _crouchingTimer = value;
		}
	}
}