using System;
using Runtime.Common.Services.Pause;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Runtime.Common.Services.Input
{
	public class InputHandler : IInputHandler, IDisposable, IPausable
	{
		PlayerInputActions _input;

		public void Dispose()
		{
			if (_input != null)
			{
				_input.Disable();
				_input.Dispose();
				_input = null;
			}
		}

		// Gameplay - Player
		public event Action<bool> RunInputPressed = delegate { };
		public event Action FlashlightInputPressed = delegate { };
		public event Action CrouchInputPressed = delegate { };
		public event Action<Vector2> PlayerMoveInputChanged = delegate { };
		public event Action<bool> JumpInputPressed = delegate { };
		public event Action<Vector2> RotateInputChanged = delegate { };
		public event Action InteractPerformed = delegate { };

		public PlayerInputActions Input => _input ??= new PlayerInputActions();
		
		public void Init()
		{
			Input.Player.Run.performed += _ => RunInputPressed?.Invoke(true);
			Input.Player.Run.canceled += _ => RunInputPressed?.Invoke(false);

			Input.Player.Move.performed += ctx => OnPlayerMoveInputChanged(ctx.ReadValue<Vector2>());
			Input.Player.Move.canceled += ctx => OnPlayerMoveInputChanged(Vector2.zero);

			Input.Player.Look.performed += ctx => OnRotateInputChanged(ctx.ReadValue<Vector2>());
			Input.Player.Interact.performed += _ => InteractPerformed?.Invoke();

			Input.Player.Jump.performed += _ => JumpInputPressed?.Invoke(true);
			Input.Player.Jump.canceled += _ => JumpInputPressed?.Invoke(false);

			Input.Player.Flashlight.performed += _ => FlashlightInputPressed?.Invoke();

			Input.Player.Crouch.performed += _ => CrouchInputPressed?.Invoke();
		}

		public void Stop() => Disable();
		public void Resume() => Enable();

		public void Enable()
		{
			Input.Enable();
		}

		public void Disable()
		{
			Input.Disable();
		}

		void OnRotateInputChanged(Vector2 direction)
		{
			RotateInputChanged?.Invoke(direction);
		}

		void OnPlayerMoveInputChanged(Vector2 direction)
		{
			PlayerMoveInputChanged?.Invoke(direction);
		}
	}
}