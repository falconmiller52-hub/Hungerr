using System.Collections.Generic;
using Runtime.Features.Sounds;
using UnityEngine;

namespace Runtime.Common.Services.Audio
{
	public class AudioService : IAudioService
	{
		private readonly SoundEmitter.Pool _pool;
		private readonly List<SoundEmitter> _activeEmitters = new();
    
		private SoundEmitter _currentMusic;
		private Vector2 _defaultPitchRange = new Vector2(1f, 1f);

		public AudioService(SoundEmitter.Pool pool)
		{
			_pool = pool;
		}

		public void PlaySfx(SoundData data, Vector3 position = default)
		{
			SoundEmitter emitter = _pool.Spawn();
			emitter.transform.position = position;

			float pitch = 1f;

			if (data.PitchRange != _defaultPitchRange)
				pitch = Random.Range(data.PitchRange.x, data.PitchRange.y);
			
			emitter.Play(data, e => {
				_activeEmitters.Remove(e);
				_pool.Despawn(e);
			}, pitch);
        
			_activeEmitters.Add(emitter);
		}

		public void PlayMusic(SoundData data, float fadeDuration = 1f)
		{
			// Здесь логика Crossfade (плавного перехода)
			// Можно использовать DOTween для затухания старого _currentMusic
			// и появления нового из пула.
		}
	}
}