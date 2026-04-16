using System;
using UnityEngine;

namespace Runtime.Common.Services.Input
{
	public interface IInputHandler
	{
		event Action<Vector2> RotateInputChanged;
		event Action<Vector2> PlayerMoveInputChanged;
		event Action<bool> JumpInputPressed;
		event Action InteractPerformed;
		event Action<bool> RunInputPressed;
		event Action FlashlightInputPressed;
		event Action CrouchInputPressed;
		event Action DialogSkipInputPressed;
		event Action ExitInputPressed;

		void Enable();
		void Disable();
	}
}