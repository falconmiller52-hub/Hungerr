using Cinemachine;
using Runtime.Common.Services.Pause;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Other
{
	public class PlayerCamera : MonoBehaviour, IPausable
	{
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

			_pauseController.Add(this);
		}

		public void Stop()
			=> _cinemachineVirtualCamera.enabled = false;

		public void Resume()
			=> _cinemachineVirtualCamera.enabled = true;
	}
}