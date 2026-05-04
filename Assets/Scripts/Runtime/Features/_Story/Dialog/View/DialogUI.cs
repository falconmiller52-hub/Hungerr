using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.Features._Story.Dialog.View
{
	public class DialogUI : MonoBehaviour
	{
		[SerializeField] private GameObject _dialogPanel;
		[SerializeField] private TextMeshProUGUI _dialogTmp;
		[SerializeField] private List<Button> _dialogButtons;

		private StorySystem _storySystem;

		[Inject]
		public void Construct(StorySystem storySystem)
		{
			_storySystem = storySystem;
		}

		private void OnEnable()
		{
			_storySystem.OnNewStoryLine += SetText;
			_storySystem.OnNewDialogChoices += SetChoices;
			_storySystem.OnStoryEnded += HideStory;
			_storySystem.OnDialogStoryStarted += ShowStory;

			foreach (var dialogButton in _dialogButtons)
			{
				dialogButton.onClick.AddListener(() => MakeChoice(_dialogButtons.IndexOf(dialogButton)));
			}
		}

		public void OnDisable()
		{
			_storySystem.OnNewStoryLine -= SetText;
			_storySystem.OnNewDialogChoices -= SetChoices;
			_storySystem.OnStoryEnded -= HideStory;
			_storySystem.OnDialogStoryStarted -= ShowStory;

			foreach (var button in _dialogButtons)
			{
				button.onClick.RemoveAllListeners();
			}
		}

		private void SetText(string text)
		{
			_dialogTmp.text = text;
		}

		private void SetChoices(List<Choice> choices)
		{
			for (int i = 0; i < _dialogButtons.Count; i++)
			{
				if (i < choices.Count)
				{
					_dialogButtons[i].gameObject.SetActive(true);
					_dialogButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i].text;
				}
				else
					_dialogButtons[i].gameObject.SetActive(false);
			}
		}

		private void MakeChoice(int index)
		{
			_storySystem.SetChoiceIndex(index);
			HideButtons();
		}

		private void ShowStory()
		{
			_dialogTmp.text = "";

			_dialogPanel.SetActive(true);
		}

		private void HideButtons()
		{
			foreach (var dialogButton in _dialogButtons)
			{
				dialogButton.gameObject.SetActive(false);
			}
		}

		private void HideStory()
		{
			_dialogPanel.SetActive(false);
			_dialogTmp.text = "";
			HideButtons();
		}
	}
}