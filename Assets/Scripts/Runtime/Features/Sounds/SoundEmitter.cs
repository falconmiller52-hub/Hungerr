using System;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Sounds
{
	/// <summary>
	/// компонент для воспроизведения звука, включается на время исполнения и потом выключается (работает через пул зенжекта) 
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public partial class SoundEmitter : MonoBehaviour
	{
		private AudioSource _source;
		private SoundData _data;
		private Action<SoundEmitter> _onFinished;

		private void Awake() => _source = GetComponent<AudioSource>();

		public float Volume
		{
			get => _source.volume;
			set => _source.volume = value;
		}
		
		public SoundData Data => _data;
		
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

			_data = data;
			_source.Play();

			if (!data.Loop)
			{
				float duration = data.Clip.length / Mathf.Abs(pitch);
				Invoke(nameof(NotifyFinished), duration);
			}
		}

		private void NotifyFinished()
		{
			_onFinished?.Invoke(this);
			_onFinished = null;
		}

		public void Stop()
		{
			CancelInvoke(nameof(NotifyFinished));
			_source.Stop();
			NotifyFinished();
		}
	}
	
	public partial class SoundEmitter { public class Pool : MonoMemoryPool<SoundEmitter> { } }
}