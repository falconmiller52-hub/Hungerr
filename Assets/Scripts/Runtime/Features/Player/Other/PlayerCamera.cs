using Cinemachine;
using Runtime.Common.Services.Pause;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Other
{
	public class PlayerCamera : MonoBehaviour, IPausable
	{
		private CinemachinePOV _pov;
		private CinemachineVirtualCamera _cinemachineVirtualCamera;
		private IPauseController _pauseController;

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

		public void Stop()
		{
			_pov.m_HorizontalAxis.m_InputAxisName = "";
			_pov.m_VerticalAxis.m_InputAxisName = "";
		}

		public void Resume()
		{
			_pov.m_HorizontalAxis.m_InputAxisName = "Mouse X";
			_pov.m_VerticalAxis.m_InputAxisName = "Mouse Y";
		}
	}
}