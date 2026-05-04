using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using Runtime.Features.Sounds;
using UnityEngine;

namespace Runtime.Features.Player.Movement
{
	[RequireComponent(typeof(PlayerMovement))]
	public class PlayerMovementStepSound : MonoBehaviour
	{
		[SerializeField, Label("Standard Step Sound")]
		private EventReference _standartdStepEvent;

		private PlayerMovement _playerMovement;
		private EventInstance _stepInstance;

		private void OnDestroy()
		{
			if (_stepInstance.isValid())
			{
				_stepInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				_stepInstance.release();
			}
		}

		private void Awake()
		{
			_playerMovement = GetComponent<PlayerMovement>();
		}

		private void Update()
		{
			//SetSoundStepBySurface();
			
			_stepInstance.getPlaybackState(out PLAYBACK_STATE state);
			
			Debug.Log($"Current state: [{state}]");
		}

		public void StartMoveSound()
		{
			if (!_stepInstance.isValid())
			{
				_stepInstance = RuntimeManager.CreateInstance(_standartdStepEvent);
				_stepInstance.start();
				
				Debug.Log("Creat and start sound step event");
				
				return;
			}

			_stepInstance.getPlaybackState(out PLAYBACK_STATE state);

			if (state == PLAYBACK_STATE.STOPPED)
			{
				Debug.Log("start again sound step event");
				_stepInstance.start();
			}
		}

		public void StopMoveSound()	
		{
			if (!_stepInstance.isValid())		
				return;
			
			_stepInstance.getPlaybackState(out PLAYBACK_STATE state);

			if (state == PLAYBACK_STATE.PLAYING)
			{
				Debug.Log("Stop sound step event");
				_stepInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}

		private void SetSoundStepBySurface()
		{
			if (!_stepInstance.isValid()) return;
			
			var ray = new Ray(_playerMovement.GroundCheck.position, -transform.up);

			Physics.Raycast(ray, out RaycastHit hit, _playerMovement.GroundCheckDistance);

			if (hit.collider &&
			    hit.collider.gameObject.TryGetComponent<SurfaceMaterialSoundHolder>(out var surfaceMaterialHolder))
			{
				surfaceMaterialHolder.SetSurfaceSoundEvent(_stepInstance);
			}
		}
	}
}