using Runtime.Features.Sounds;
using UnityEngine;

namespace Runtime.Common.Services.Audio
{
	public interface IAudioService
	{
		void PlaySfx(SoundData data, Vector3 position = default);
		void PlayMusic(SoundData data, float fadeDuration = 1f);
	}
}