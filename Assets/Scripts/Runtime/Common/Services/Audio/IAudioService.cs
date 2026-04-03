using System;
using Runtime.Features.Sounds;
using UnityEngine;

namespace Runtime.Common.Services.Audio
{
	public interface IAudioService
	{
		void PlaySfx(SoundData data, Vector3 position = default, Action<SoundData> onEnd = null);
		void PlayMusic(SoundData data, float fadeDuration = 1f);
		void StopPlaying(SoundData data);
	}
}