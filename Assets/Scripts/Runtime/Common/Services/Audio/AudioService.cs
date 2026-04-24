using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Runtime.Common.Services.Audio
{
	public class AudioService : IAudioService
	{
		public void PlayOneShotAt(EventReference eventRef, Vector3 position = default)
			=> RuntimeManager.PlayOneShot(eventRef, position);

		public void PlayOneShot2D(EventReference eventRef)
			=> RuntimeManager.PlayOneShot(eventRef);

		public EventInstance PlaySound(EventReference eventRef, Vector3 position = default)
		{
			var instance = RuntimeManager.CreateInstance(eventRef);
			instance.set3DAttributes(position.To3DAttributes());
			instance.start();

			return instance;
		}

		public void StopSound(EventInstance instance, STOP_MODE mode)
		{
			if (instance.isValid())
			{
				instance.stop(mode);
				instance.release();
			}
		}
	}
}