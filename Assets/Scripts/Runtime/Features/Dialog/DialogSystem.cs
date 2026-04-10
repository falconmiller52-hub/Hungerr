using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

namespace _GAME._1_Scripts.INK
{
	public class DialogSystem : MonoBehaviour
	{
		[SerializeField] private float _typeWriterSpeed;

		// TODO: Переделать на eventbus 
		public event Action<string> OnNewDialogLine;
		public event Action<List<Choice>> OnNewDialogChoices;
		public event Action OnStoryStarted;
		public event Action OnStoryEnded;

		private Story _story;

		private Coroutine _startDialogRoutine;
		private Coroutine _typeWriterRoutine;

		private int _choiceIndex = -1;
		private string _currentLine;

		private void Update()
		{
			StopDialog();
		}

		public void SetChoiceIndex(int index)
		{
			_choiceIndex = index;
		}

		public void StartStory(TextAsset storyJsonInk)
		{
			_story = new Story(storyJsonInk.text);

			if (_startDialogRoutine == null)
			{
				_startDialogRoutine = StartCoroutine(DialogRoutine());
				OnStoryStarted?.Invoke();
			}
			else
				Debug.Log("<color=red>Диалог пытался запуститься в то время пока другой ещё не закончился</color>");
		}

		private IEnumerator DialogRoutine()
		{
			while (true)
			{
				while (_story.canContinue)
				{
					yield return null;

					if (_typeWriterRoutine == null)
					{
						_currentLine = _story.Continue();
						_typeWriterRoutine = StartCoroutine((TypeWriterRoutine(_currentLine)));
					}
					else
					{
						StopTypeWriterEffect();
					}

					//HandleTags(_story);
					
					//TODO: Переделать на InputHandler
					yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));
				}

				if (_story.currentChoices.Count > 0)
				{
					if (_typeWriterRoutine != null)
					{
						StopTypeWriterEffect();
					}

					OnNewDialogChoices?.Invoke(_story.currentChoices);

					yield return new WaitUntil(() => { return _choiceIndex >= 0; });

					Choose(_choiceIndex, _story);
				}
				else
				{
					EndStory();
					_startDialogRoutine = null;
					break;
				}
			}
		}

		private void StopTypeWriterEffect()
		{
			if (_typeWriterRoutine != null)
			{
				StopCoroutine(_typeWriterRoutine);

				OnNewDialogLine?.Invoke(_currentLine);

				_typeWriterRoutine = null;
			}
		}

		private void StopDialog()
		{
			//TODO: Переделать на InputHandler

			if (_startDialogRoutine != null)
			{
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					StopTypeWriterEffect();
				
					EndStory();
				
					StopCoroutine(_startDialogRoutine);
					_startDialogRoutine = null;
				}
			}
		}

		private void Choose(int index, Story story)
		{
			story.ChooseChoiceIndex(index);
			_choiceIndex = -1;
		}

		private void EndStory()
		{
			Debug.Log("=== END OF STORY ===");
			OnStoryEnded?.Invoke();
		}

		private IEnumerator TypeWriterRoutine(string line)
		{
			string currentLine = "";
			foreach (char symbol in line)
			{
				currentLine += symbol;

				OnNewDialogLine?.Invoke(currentLine);

				yield return new WaitForSeconds(_typeWriterSpeed);
			}

			_typeWriterRoutine = null;
		}

		// Сюда пока что не смотреть :)
		private void HandleTags(Story _story)
		{
			foreach (var storyCurrentTag in _story.currentTags)
			{
				string[] tags = storyCurrentTag.Split(':');

				string key = tags[0];
				string value = tags[1];

				switch (key)
				{
					case "sound":
					{
						Debug.Log($"Play sound {value}");
						break;
					}
					case "camera":
					{
						Debug.Log($"Camera is {value}");
						break;
					}
				}
			}
		}
	}
}