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
		public event Action FlashlightInputPressed = delegate { };
		public event Action CrouchInputPressed = delegate { };
		public event Action DialogSkipInputPressed =  delegate { };
		public event Action ExitInputPressed = delegate { };
		public event Action InventoryTriggerPressed = delegate { };
		public event Action InventoryUsePressed = delegate { };
		public event Action<Vector2> PlayerMoveInputChanged = delegate { };
		public event Action<Vector2> RotateInputChanged = delegate { };
		public event Action InteractPerformed = delegate { };

		public PlayerInputActions Input => _input ??= new PlayerInputActions();
		
		public void Init()
		{
			Input.Gameplay.Run.performed += _ => RunInputPressed?.Invoke(true);
			Input.Gameplay.Run.canceled += _ => RunInputPressed?.Invoke(false);

			Input.Gameplay.Move.performed += ctx => OnPlayerMoveInputChanged(ctx.ReadValue<Vector2>());
			Input.Gameplay.Move.canceled += ctx => OnPlayerMoveInputChanged(Vector2.zero);

			Input.Gameplay.Look.performed += ctx => OnRotateInputChanged(ctx.ReadValue<Vector2>());
			Input.Gameplay.Interact.performed += _ => InteractPerformed?.Invoke();

			Input.Gameplay.Flashlight.performed += _ => FlashlightInputPressed?.Invoke();

			Input.Gameplay.Crouch.performed += _ => CrouchInputPressed?.Invoke();
			Input.Gameplay.InventoryTrigger.performed += _ => InventoryTriggerPressed?.Invoke();
			
			Input.UI.DialogSkip.performed += _ => DialogSkipInputPressed?.Invoke();
			Input.UI.Exit.performed += _ => ExitInputPressed?.Invoke();
			Input.UI.Use.performed += _ => InventoryUsePressed?.Invoke();
		}
		
		public void Enable()
		{
			Input.Enable();
		}

		public void Disable()
		{
			Input.Disable();
		}

		public void SwitchToUIMap()
		{
			Input.Gameplay.Disable();
			Input.UI.Enable();
		}
		
		public void SwitchToGameplayMap()
		{
			Input.UI.Disable();
			Input.Gameplay.Enable();
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