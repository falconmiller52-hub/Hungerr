using System;
using Runtime.Common.Services.Input;
using UnityEngine;
using Zenject;

namespace Runtime.Common.Services.Pause
{
	public class PauseRegistry : MonoBehaviour
	{
		private IPauseController _pauseController;
		private InputHandler _input;

		[Inject]
		private void Construct(IPauseController pauseController, InputHandler input)
		{
			_pauseController = pauseController;
			_input = input;
		}
		
		private void Start()
		{
			_pauseController.Add(_input);
		}
	}
}