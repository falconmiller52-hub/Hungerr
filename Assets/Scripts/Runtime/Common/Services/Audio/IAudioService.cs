using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Runtime.Common.Services.Audio
{
	public interface IAudioService
	{
		void PlayOneShotAt(EventReference eventRef, Vector3 position = default);
		void PlayOneShot2D(EventReference eventRef);
		EventInstance PlaySound(EventReference eventRef, Vector3 position = default);
		void StopSound(EventInstance instance, STOP_MODE mode);
	}
}