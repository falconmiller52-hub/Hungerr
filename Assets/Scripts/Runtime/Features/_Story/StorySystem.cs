using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Ink.Runtime;
using Runtime.Common.Services.Audio.Sound;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.Pause;
using UnityEngine;
using Zenject;

namespace Runtime.Features._Story
{
	public class StorySystem : MonoBehaviour
	{
		[SerializeField] private float _typeWriterSpeed;

		public event Action<string> OnNewStoryLine;
		public event Action<List<Choice>> OnNewDialogChoices;
		public event Action OnDialogStoryStarted;
		public event Action OnMonologStoryStarted;
		public event Action OnStoryEnded;

		private Story _story;
		private StoryTagSystem _storyTagSystem;

		private IInputHandler _inputHandler;
		private ISoundService _soundService;
		private IPauseController _pauseController;

		private Coroutine _startDialogRoutine;
		private Coroutine _typeWriterRoutine;

		private EventReference _currentEventReference;

		private int _choiceIndex = -1;
		private string _currentLine;
		private bool _isStoryStart;
		private bool _canSkip;
		private bool _isStoryText;

		[Inject]
		private void Construct(IPauseController pauseController, IInputHandler inputHandler, ISoundService soundService
						, StoryTagSystem storyTagSystem)
		{
			_pauseController = pauseController;
			_inputHandler = inputHandler;
			_soundService = soundService;
			_storyTagSystem = storyTagSystem;
		}

		private void OnEnable()
		{
			_inputHandler.DialogSkipInputPressed += GetMouseInteract;
			_inputHandler.ExitInputPressed += StopDialog;
			_storyTagSystem.OnTagStoryEnd += EndStory;
		}

		private void OnDisable()
		{
			_inputHandler.DialogSkipInputPressed -= GetMouseInteract;
			_inputHandler.ExitInputPressed -= StopDialog;
			_storyTagSystem.OnTagStoryEnd -= EndStory;
		}

		public void SetChoiceIndex(int index)
			=> _choiceIndex = index;

		public void StartStory(Story story, EventReference eventReference, bool isMonolog = false)
		{
			if (_startDialogRoutine != null)
			{
				Debug.Log("<color=red>Диалог пытался запуститься в то время пока другой ещё не закончился</color>");
				return;
			}
			
			_currentEventReference = eventReference;

			if (!isMonolog)
			{
				_pauseController.PerformStop();
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				
				_inputHandler.SwitchToUIMap();
			}

			_isStoryStart = true;
			_story = story;

			if (_startDialogRoutine == null)
			{
				_startDialogRoutine = StartCoroutine(DialogRoutine());

				if (isMonolog)
					OnMonologStoryStarted?.Invoke();
				else
					OnDialogStoryStarted?.Invoke();
			}
		}

		private IEnumerator DialogRoutine()
		{
			_story.ChoosePathString("Main");
			while (_story != null)
			{
				// Показываем текст диалога, ждём пока пользователь не нажмёт на ЛКМ
				while (_story.canContinue)
				{
					_isStoryText = true;

					if (_typeWriterRoutine == null)
					{
						_currentLine = _story.Continue();

						if (_story.currentTags.Count > 0)
							_storyTagSystem.ParseTag(_story.currentTags);

						_typeWriterRoutine = StartCoroutine((TypeWriterRoutine(_currentLine)));
					}
					else
						StopTypeWriterEffect();

					yield return new WaitUntil(() => _canSkip);
					_canSkip = false;
				}

				_isStoryText = false;

				// Когда текст закончился показываем варианты выбора
				if (_story.currentChoices.Count > 0)
				{
					if (_typeWriterRoutine != null)
						StopTypeWriterEffect();

					OnNewDialogChoices?.Invoke(_story.currentChoices);

					yield return new WaitUntil(() => _choiceIndex >= 0);

					Choose(_choiceIndex, _story);
				}
				// Когда ни текста ни выборов больше нет заканчиваем диалог.
				else
				{
					_isStoryText = true;

					yield return new WaitUntil(() => _canSkip);

					_canSkip = false;
					_isStoryText = false;

					EndStory();
					_startDialogRoutine = null;
					break;
				}
			}
		}

		private void GetMouseInteract()
		{
			// Если мы в диалоги и в диалоге есть ещё строки, а не выборы
			if (_isStoryStart && _isStoryText)
				_canSkip = true;
		}

		private void StopTypeWriterEffect()
		{
			if (_typeWriterRoutine != null)
			{
				StopCoroutine(_typeWriterRoutine);

				OnNewStoryLine?.Invoke(_currentLine);

				_typeWriterRoutine = null;
			}
		}

		private void StopDialog()
		{
			if (_startDialogRoutine != null && _isStoryStart)
			{
				StopTypeWriterEffect();

				StopCoroutine(_startDialogRoutine);
				_startDialogRoutine = null;

				EndStory();
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

			if (_startDialogRoutine != null)
			{
				StopCoroutine(_startDialogRoutine);
				_startDialogRoutine = null;
			}

			_pauseController.PerformResume();
			_inputHandler.SwitchToPlayerMap();
			
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

				if (!_currentEventReference.IsNull)
					_soundService.PlayOneShot2D(_currentEventReference);

				OnNewStoryLine?.Invoke(currentLine);

				yield return new WaitForSeconds(_typeWriterSpeed);
			}

			// Когда эффект печатанья закончился, если следующая строка это выбор, то показываем её сразу, 
			// не дожидаясь когда игрок нажмёт MouseInteract

			if (_story != null)
			{
				if (!_story.canContinue)
					_canSkip = true;
			}

			_typeWriterRoutine = null;
		}
	}
}