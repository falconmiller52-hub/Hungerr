using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.Pause;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Dialog
{
	public class DialogSystem : MonoBehaviour
	{
		[SerializeField] private float _typeWriterSpeed;

		public event Action<string> OnNewDialogLine;
		public event Action<List<Choice>> OnNewDialogChoices;
		public event Action OnStoryStarted;
		public event Action OnStoryEnded;

		private IPauseController _pauseController;
		private Story _story;
		private IInputHandler _inputHandler;

		private Coroutine _startDialogRoutine;
		private Coroutine _typeWriterRoutine;

		private int _choiceIndex = -1;
		private string _currentLine;
		private bool _isStoryStart;
		private bool _canSkip;
		private bool _isDialogText;

		[Inject]
		private void Construct(IPauseController pauseController, IInputHandler inputHandler)
		{
			_pauseController = pauseController;
			_inputHandler = inputHandler;
		}

		private void OnEnable()
		{
			_inputHandler.DialogSkipInputPressed += GetMouseInteract;
			_inputHandler.ExitInputPressed += StopDialog;
		}

		private void OnDisable()
		{
			_inputHandler.DialogSkipInputPressed -= GetMouseInteract;
			_inputHandler.ExitInputPressed -= StopDialog;
		}

		public void SetChoiceIndex(int index)
			=> _choiceIndex = index;


		public void StartStory(Story story, bool isMonolog = false)
		{
			if (!isMonolog)
			{
				_pauseController.PerformStop();
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}

			_isStoryStart = true;
			_story = story;

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
			_story.ChoosePathString("Main");
			while (_story != null)
			{
				// Показываем текст диалога, ждём пока пользователь не нажмёт на ЛКМ
				while (_story.canContinue)
				{
					_isDialogText = true;
					if (_typeWriterRoutine == null)
					{
						_currentLine = _story.Continue();
						_typeWriterRoutine = StartCoroutine((TypeWriterRoutine(_currentLine)));
					}
					else
					{
						StopTypeWriterEffect();
					}

					yield return new WaitUntil(() => _canSkip);
					_canSkip = false;
				}

				_isDialogText = false;

				// Когда текст закончился показываем варианты выбора
				if (_story.currentChoices.Count > 0)
				{
					if (_typeWriterRoutine != null)
					{
						StopTypeWriterEffect();
					}

					OnNewDialogChoices?.Invoke(_story.currentChoices);

					yield return new WaitUntil(() => _choiceIndex >= 0);

					Choose(_choiceIndex, _story);
				}
				// Когда ни текста ни выборов больше нет заканчиваем диалог.
				else
				{
					EndStory();
					_startDialogRoutine = null;
					break;
				}
			}
		}

		private void GetMouseInteract()
		{
			// Если мы в диалоги и в диалоге есть ещё строки, а не выборы
			if (_isStoryStart && _isDialogText)
				_canSkip = true;
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
			if (_startDialogRoutine != null && _isStoryStart)
			{
				StopTypeWriterEffect();

				EndStory();

				StopCoroutine(_startDialogRoutine);
				_startDialogRoutine = null;
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
			_story = null;
			_isStoryStart = false;
			_pauseController.PerformResume();
			OnStoryEnded?.Invoke();
			StopTypeWriterEffect();

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
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

			// Когда эффект печатанья закончился, если следующая строка это выбор, то показываем её сразу, 
			// не дожидаясь когда игрок нажмёт MouseInteract
			if (!_story.canContinue)
			{
				_canSkip = true;
			}

			_typeWriterRoutine = null;
		}
	}
}