using Cinemachine;
using FMODUnity;
using Ink.Runtime;
using Runtime.Features._Story;
using Runtime.Features.Interactable;
using UnityEngine;
using Zenject;

namespace Runtime.Features.NPC
{
	
	[SelectionBase]
	public class Npc : MonoBehaviour, IInteractable
	{
		[SerializeField] [Tooltip("Файл с диалогом NPC")]
		private TextAsset _dialogJson;

		[SerializeField] [Tooltip("Камера на которую будет смещен фокус при старте диалога")]
		private CinemachineVirtualCamera _cinemachineVirtualCamera;

		[SerializeField] [Tooltip("Звук диалога NPC")]
		private EventReference _npcDialogSound;

		private StorySystem _storySystem;
		private Story _story;

		[Inject]
		public void Construct(StorySystem storySystem)
		{
			_storySystem = storySystem;
		}

		private void Start()
		{
			_story = new Story(_dialogJson.text);

			_storySystem.OnStoryEnded += DisableNpcCamera;
		}

		private void OnDestroy()
		{
			_storySystem.OnStoryEnded -= DisableNpcCamera;
		}

		public void Interact()
		{
			StartDialog();
		}

		private void StartDialog()
		{
			_storySystem.StartStory(_story, _npcDialogSound);
			EnableNpcCamera();
		}

		private void EnableNpcCamera()
			=> _cinemachineVirtualCamera.Priority = 100;

		private void DisableNpcCamera()
			=> _cinemachineVirtualCamera.Priority = 0;
	}
}