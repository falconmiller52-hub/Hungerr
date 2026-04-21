using NaughtyAttributes;
using Runtime.Common.Services.Audio;
using Runtime.Common.Services.Input;
using Runtime.Features.Player.Movement;
using Runtime.Features.Sounds;
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
		private SoundData _flashlightTurnOnSound;

		[Space, SerializeField, Label("Flashlight OFF Sound ")]
		private SoundData _flashlightTurnffSound;

		private IInputHandler _inputHandler;
		private IAudioService _audioService;
		private bool _isEnabled = false;

		[Inject]
		private void Construct(IInputHandler inputHandler, IAudioService audioService)
		{
			_inputHandler = inputHandler;
			_audioService = audioService;
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
			_audioService.PlaySfx(sound, transform.position);
		}
	}
}