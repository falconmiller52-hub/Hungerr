using System.Collections.Generic;
using FMODUnity;
using Runtime.Common.Services.Audio;
using Runtime.Common.Services.Audio.Sound;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Runtime.Features._Story.Dialog.View
{
	public class DialogUiButtonSound : MonoBehaviour, IPointerEnterHandler
	{
		[SerializeField] [Tooltip("Звук при наведении на кнопку")]
		private EventReference _buttonHoverSound;

		[SerializeField] [Tooltip("Звук при нажатии на кнопку")]
		private EventReference _buttonSelectSound;

		[SerializeField] private List<Button> _dialogButtons;

		private ISoundService _soundService;

		[Inject]
		private void Construct(ISoundService soundService)
		{
			_soundService = soundService;
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
			_soundService.PlayOneShot2D(_buttonHoverSound);
		}

		private void PlaySoundButtonSelected()
			=> _soundService.PlayOneShot2D(_buttonSelectSound);
	}
}