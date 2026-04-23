using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Runtime.Common.Services.Audio
{
	public interface IAudioService
	{
		EventInstance PlaySound(EventReference eventRef, Vector3 position = default);
		void StopSound(EventInstance instance, STOP_MODE mode);
	}
}