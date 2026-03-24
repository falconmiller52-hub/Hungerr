using System;
using UnityEngine;

namespace Runtime.Common.Services.Input
{
	public class InputHandler : IInputHandler, IDisposable
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
		public event Action<Vector2> PlayerMoveInputChanged = delegate { };
		public event Action<bool> JumpInputPressed = delegate { };
		public event Action<Vector2> RotateInputChanged = delegate { };
		public event Action InteractPerformed = delegate { };
		
		public PlayerInputActions Input => _input ??= new PlayerInputActions();

		public void Enable()
		{
			Input.Player.Run.performed += _ => RunInputPressed?.Invoke(true);
			Input.Player.Run.canceled += _ => RunInputPressed?.Invoke(false);

			Input.Player.Move.performed += ctx => OnPlayerMoveInputChanged(ctx.ReadValue<Vector2>());
			Input.Player.Move.canceled += ctx => OnPlayerMoveInputChanged(Vector2.zero);
			
			Input.Player.Look.performed += ctx => OnRotateInputChanged(ctx.ReadValue<Vector2>());
			Input.Player.Interact.performed += _ => InteractPerformed?.Invoke();

			Input.Player.Jump.performed += _ => JumpInputPressed?.Invoke(true);
			Input.Player.Jump.canceled += _ => JumpInputPressed?.Invoke(false);

			Input.Enable();
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