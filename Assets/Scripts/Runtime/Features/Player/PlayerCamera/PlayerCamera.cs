using Cinemachine;
using Runtime.Common.Services.Pause;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Runtime.Features.Player.PlayerCamera
{
	[RequireComponent(typeof(CinemachineVirtualCamera))]
	public class PlayerCamera : MonoBehaviour, IPausable
	{
		private CinemachinePOV _pov;
		private CinemachineVirtualCamera _cinemachineVirtualCamera;
		private IPauseController _pauseController;
		private bool _isPaused;

		[Inject]
		private void Construct(IPauseController pauseController)
		{
			_pauseController = pauseController;
		}

		private void Awake()
		{
			_cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
			_pov = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>();

			_pauseController.Add(this);
		}

		private void Update()
		{
			if (_isPaused)
				return;
			
			var mouse = Mouse.current;
			Vector2 mouseDelta = mouse.delta.ReadValue();

			_pov.m_HorizontalAxis.Value += mouseDelta.x * Time.deltaTime;
			_pov.m_VerticalAxis.Value -= mouseDelta.y * Time.deltaTime;
		}

		public void Stop()
		{
			_isPaused = true;
			
			_pov.m_HorizontalAxis.m_InputAxisName = "";
			_pov.m_VerticalAxis.m_InputAxisName = "";

			_pov.m_HorizontalAxis.m_InputAxisValue = 0;
			_pov.m_VerticalAxis.m_InputAxisValue = 0;
		}

		public void Resume()
		{
			_pov.m_HorizontalAxis.m_InputAxisName = "Mouse X";
			_pov.m_VerticalAxis.m_InputAxisName = "Mouse Y";
			
			_isPaused = false;
		}
	}
}