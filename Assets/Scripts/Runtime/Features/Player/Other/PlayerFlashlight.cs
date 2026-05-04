using FMODUnity;
using NaughtyAttributes;
using Runtime.Common.Services.Audio.Sound;
using Runtime.Common.Services.Input;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Other
{
	public class PlayerFlashlight : MonoBehaviour
	{
		[SerializeField, Label("Flashlight Object")]
		private Light _flashlightObject;

		[SerializeField, Label("Flashlight Intensity")]
		private float _intensity = 1f;

		[Space, SerializeField, Label("Flashlight ON Sound ")]
		private EventReference _flashlightTurnOnSound;

		[Space, SerializeField, Label("Flashlight OFF Sound ")]
		private EventReference _flashlightTurnffSound;

		private IInputHandler _inputHandler;
		private ISoundService _soundService;
		private bool _isEnabled = false;

		[Inject]
		private void Construct(IInputHandler inputHandler, ISoundService soundService)
		{
			_inputHandler = inputHandler;
			_soundService = soundService;
		}

		private void OnEnable()
		{
			if (_inputHandler == null)
			{
				Debug.LogError("PlayerFlashlight::OnEnable() No Input Handler was assigned");
				return;
			}

			_inputHandler.FlashlightInputPressed += Toggle;
		}

		private void OnDisable()
		{
			if (_inputHandler == null)
			{
				Debug.LogError("PlayerFlashlight::OnDisable() No Input Handler was assigned");
				return;
			}

			_inputHandler.FlashlightInputPressed -= Toggle;
		}

		public void Toggle()
		{
			_isEnabled = !_isEnabled;

			// Устанавливаем интенсивность (если включен — берем конфиг, если нет — 0)
			_flashlightObject.intensity = _isEnabled ? _intensity : 0f;

			var sound = _isEnabled ? _flashlightTurnOnSound : _flashlightTurnffSound;
			_soundService.PlaySound(sound, transform.position);
		}
	}
}