using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Features.Sounds;
using UnityEngine;
using Random = UnityEngine.Random;

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

		public void PlaySfx(SoundData data, Vector3 position, Action<SoundData> onEnd = null)
		{
			if (data == null)
			{
				Debug.LogError("AudioService::PlaySfx() data is null");
				return;
			}
			
			SoundEmitter emitter = _pool.Spawn();
			emitter.transform.position = position;

			float pitch = 1f;

			if (data.PitchRange != _defaultPitchRange)
				pitch = Random.Range(data.PitchRange.x, data.PitchRange.y);
			
			emitter.Play(data, e => {
				_activeEmitters.Remove(e);
				_pool.Despawn(e);
				onEnd?.Invoke(data);
			}, pitch);
        
			_activeEmitters.Add(emitter);
		}

		public void PlayMusic(SoundData data, float fadeDuration = 1f)
		{
		}

		public void StopPlaying(SoundData data)
		{
			for (int i = _activeEmitters.Count - 1; i >= 0; i--)
			{
				var emitter = _activeEmitters[i];
        
				if (emitter.Data == data)
				{
					emitter.Stop();
				}
			}
		}
	}
}