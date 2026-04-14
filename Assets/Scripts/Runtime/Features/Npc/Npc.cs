using System;
using Ink.Runtime;
using Runtime.Features.Dialog;
using Runtime.Features.Interactable;
using UnityEngine;
using Zenject;

namespace Runtime.Features.NPC
{
	// Тестовая вариация NPC для тестов диалоговой системы
	public class Npc : MonoBehaviour, IInteractable
	{
		[SerializeField] [Tooltip("Файл с диалогом NPC")] private TextAsset _dialogJson;
		
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
		}

		public void Interact()
		{
			StartDialog();
		}

		private void StartDialog() => _dialogSystem.StartStory(_story);
	}
}