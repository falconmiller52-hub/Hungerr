using System.Collections;
using Cinemachine;
using NaughtyAttributes;
using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.PlayerCamera
{
	[RequireComponent(typeof(CinemachineVirtualCamera))]
	public class PlayerCameraShake : MonoBehaviour
	{
		[BoxGroup("Noise Settings")]
		[InfoBox("X = Amplitude Gain (сила тряски)\nY = Frequency Gain (скорость/частота тряски)")] 
		
		[Label("Ходьба")] [SerializeField] 
		private Vector2 _walkShake;

		[BoxGroup("Noise Settings")] [Label("Бег")] [SerializeField]
		private Vector2 _runShake;

		[BoxGroup("Noise Settings")] [Label("Вприсядку(уточкой)")] [SerializeField]
		private Vector2 _crouchShake;

		[SerializeField] [Tooltip("Время за которое происходит смена показателей тряски")]
		private float _changeShakeTime;

		private CinemachineVirtualCamera _cinemachineVirtualCamera;
		private CinemachineBasicMultiChannelPerlin _cinemachinePerlin;
		private EventBus _eventBus;
		private Coroutine _changeShakeRoutine;

		private float _t;

		[Inject]
		private void Construct(EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		private void Awake()
		{
			_cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
			_cinemachinePerlin =
							_cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		}

		private void OnEnable()
		{
			_eventBus.Subscribe(EPlayerStanceEvent.StartWalkState, ChangeCameraShakeOnWalkState);
			_eventBus.Subscribe(EPlayerStanceEvent.StartRunState, ChangeCameraShakeOnRunState);
			_eventBus.Subscribe(EPlayerStanceEvent.StartCrouchState, ChangeCameraShakeOnCrouchState);
		}

		private void OnDisable()
		{
			_eventBus.Unsubscribe(EPlayerStanceEvent.StartWalkState, ChangeCameraShakeOnWalkState);
			_eventBus.Unsubscribe(EPlayerStanceEvent.StartRunState, ChangeCameraShakeOnRunState);
			_eventBus.Unsubscribe(EPlayerStanceEvent.StartCrouchState, ChangeCameraShakeOnCrouchState);
		}

		private void ChangeCameraShakeOnWalkState()
			=> ChangeCameraShakeOnNewState(_walkShake);

		private void ChangeCameraShakeOnRunState()
			=> ChangeCameraShakeOnNewState(_runShake);

		private void ChangeCameraShakeOnCrouchState()
			=> ChangeCameraShakeOnNewState(_crouchShake);

		private void ChangeCameraShakeOnNewState(Vector2 value)
		{
			if (_changeShakeRoutine == null)
			{
				_changeShakeRoutine = StartCoroutine(ChangeCameraShakeRoutine(value));
			}
			else
			{
				StopCoroutine(_changeShakeRoutine);
				_changeShakeRoutine = null;

				_changeShakeRoutine = StartCoroutine(ChangeCameraShakeRoutine(value));
			}
		}

		private IEnumerator ChangeCameraShakeRoutine(Vector2 value)
		{
			_t = 0;

			float currentAmplitude = _cinemachinePerlin.m_AmplitudeGain;
			float currentFrequency = _cinemachinePerlin.m_FrequencyGain;

			while (_t < 1)
			{
				_t += Time.deltaTime / _changeShakeTime;

				_cinemachinePerlin.m_AmplitudeGain = Mathf.Lerp(currentAmplitude, value.x, _t);
				_cinemachinePerlin.m_FrequencyGain = Mathf.Lerp(currentFrequency, value.y, _t);

				yield return null;
			}

			_changeShakeRoutine = null;
		}
	}
}