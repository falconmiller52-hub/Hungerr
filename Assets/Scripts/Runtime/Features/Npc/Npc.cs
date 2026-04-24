using Cinemachine;
using FMODUnity;
using Ink.Runtime;
using Runtime.Features.Dialog;
using Runtime.Features.Interactable;
using UnityEngine;
using Zenject;

namespace Runtime.Features.NPC
{
	// Тестовая вариация NPC для тестов диалоговой системы
	[SelectionBase]
	public class Npc : MonoBehaviour, IInteractable
	{
		[SerializeField] [Tooltip("Файл с диалогом NPC")]
		private TextAsset _dialogJson;

		[SerializeField] [Tooltip("Камера на которую будет смещен фокус при старте диалога")]
		private CinemachineVirtualCamera _cinemachineVirtualCamera;

		[SerializeField] [Tooltip("Звук диалога NPC")]
		private EventReference _npcDialogSound;

		private DialogSystem _dialogSystem;
		private Story _story;

		[Inject]
		public void Construct(DialogSystem dialogSystem)
		{
			_dialogSystem = dialogSystem;
		}

		private void Start()
		{
			_story = new Story(_dialogJson.text);

			_dialogSystem.OnStoryEnded += DisableNpcCamera;
		}

		private void OnDestroy()
		{
			_dialogSystem.OnStoryEnded -= DisableNpcCamera;
		}

		public void Interact()
		{
			StartDialog();
		}

		private void StartDialog()
		{
			_dialogSystem.StartStory(_story, _npcDialogSound);
			EnableNpcCamera();
		}

		private void EnableNpcCamera()
			=> _cinemachineVirtualCamera.Priority = 100;

		private void DisableNpcCamera()
			=> _cinemachineVirtualCamera.Priority = 0;
	}
}