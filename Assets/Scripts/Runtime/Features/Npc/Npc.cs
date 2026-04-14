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

		[Inject]
		public void Construct(DialogSystem dialogSystem)
		{
			_dialogSystem = dialogSystem;
		}
		
		public void Interact()
		{
			StartDialog();
		}

		private void StartDialog() => _dialogSystem.StartStory(_dialogJson);
	}
}