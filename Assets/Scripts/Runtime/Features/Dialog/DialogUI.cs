using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.Features.Dialog
{
	public class DialogUI : MonoBehaviour
	{
		[SerializeField] private GameObject _dialogPanel;
		[SerializeField] private TextMeshProUGUI _dialogTmp;
		[SerializeField] private List<Button> _dialogButtons;

		private DialogSystem _dialogSystem;

		[Inject]
		public void Construct(DialogSystem dialogSystem)
		{
			_dialogSystem = dialogSystem;
		}

		private void OnEnable()
		{
			_dialogSystem.OnNewDialogLine += SetText;
			_dialogSystem.OnNewDialogChoices += SetChoices;
			_dialogSystem.OnStoryEnded += HideDialog;
			_dialogSystem.OnStoryStarted += ShowDialog;

			foreach (var dialogButton in _dialogButtons)
			{
				dialogButton.onClick.AddListener(() => MakeChoice(_dialogButtons.IndexOf(dialogButton)));
			}
		}

		public void OnDisable()
		{
			_dialogSystem.OnNewDialogLine -= SetText;
			_dialogSystem.OnNewDialogChoices -= SetChoices;
			_dialogSystem.OnStoryEnded -= HideDialog;
			_dialogSystem.OnStoryStarted -= ShowDialog;

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
			_dialogSystem.SetChoiceIndex(index);
			HideButtons();
		}

		private void ShowDialog()
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

		private void HideDialog()
		{
			_dialogPanel.SetActive(false);
			_dialogTmp.text = "";
			HideButtons();
		}
	}
}