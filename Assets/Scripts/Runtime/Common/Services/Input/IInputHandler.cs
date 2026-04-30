using System;
using UnityEngine;

namespace Runtime.Common.Services.Input
{
	public interface IInputHandler
	{
		event Action<Vector2> RotateInputChanged;
		event Action<Vector2> PlayerMoveInputChanged;
		event Action InteractPerformed;
		event Action<bool> RunInputPressed;
		event Action FlashlightInputPressed;
		event Action CrouchInputPressed;
		event Action DialogSkipInputPressed;
		event Action ExitInputPressed;
		event Action InventoryTriggerPressed;
		
		event Action InventoryGrabPressed;
		event Action InventoryReleasePressed;
		event Action InventoryUsePressed;

		void Enable();
		void Disable();
	}
}