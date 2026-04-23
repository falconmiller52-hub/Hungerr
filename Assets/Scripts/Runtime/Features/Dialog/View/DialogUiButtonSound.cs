using System.Collections.Generic;
using FMODUnity;
using Runtime.Common.Services.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Runtime.Features.Dialog.View
{
	public class DialogUiButtonSound : MonoBehaviour, IPointerEnterHandler
	{
		[SerializeField] [Tooltip("Звук при наведении на кнопку")]
		private EventReference _buttonHoverSound;

		[SerializeField] [Tooltip("Звук при нажатии на кнопку")]
		private EventReference _buttonSelectSound;

		[SerializeField] private List<Button> _dialogButtons;

		private IAudioService _audioService;

		[Inject]
		private void Construct(IAudioService audioService)
		{
			_audioService = audioService;
		}

		private void OnEnable()
		{
			foreach (var button in _dialogButtons)
			{
				button.onClick.AddListener(PlaySoundButtonSelected);
			}
		}

		private void OnDisable()
		{
			foreach (var button in _dialogButtons)
			{
				button.onClick.RemoveListener(PlaySoundButtonSelected);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_audioService.PlayOneShot2D(_buttonHoverSound);
		}

		private void PlaySoundButtonSelected()
			=> _audioService.PlayOneShot2D(_buttonSelectSound);
	}
}