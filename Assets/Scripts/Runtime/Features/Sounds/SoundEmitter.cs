using System;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Sounds
{
	[RequireComponent(typeof(AudioSource))]
	public partial class SoundEmitter : MonoBehaviour
	{
		private AudioSource _source;
		private Action<SoundEmitter> _onFinished;

		private void Awake() => _source = GetComponent<AudioSource>();

		public void Play(SoundData data, Action<SoundEmitter> onFinished, float pitch = 1)
		{
			_onFinished = onFinished;
        
			_source.clip = data.Clip;
			_source.outputAudioMixerGroup = data.MixerGroup;
			_source.volume = data.Volume;
			_source.pitch = pitch;
			_source.loop = data.Loop;
			_source.spatialBlend = data.SpatialBlend;
			_source.minDistance = data.MinDistance;
			_source.maxDistance = data.MaxDistance;
			_source.rolloffMode = data.RolloffMode;

			_source.Play();

			if (!data.Loop)
			{
				Invoke(nameof(NotifyFinished), data.Clip.length / Mathf.Abs(pitch));
			}
		}

		private void NotifyFinished() => _onFinished?.Invoke(this);
		public void Stop() => _source.Stop();
	}
	
	public partial class SoundEmitter { public class Pool : MonoMemoryPool<SoundEmitter> { } }
}