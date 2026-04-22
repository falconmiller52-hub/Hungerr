using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Runtime.Common.Services.Audio
{
	public class AudioService : IAudioService
	{
		public EventInstance PlaySound(EventReference eventRef, Vector3 position)
		{
			var instance = RuntimeManager.CreateInstance(eventRef);
			instance.set3DAttributes(position.To3DAttributes());
			instance.start();
			
			instance.release();
			
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