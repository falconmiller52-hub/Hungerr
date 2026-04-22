using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using PrimeTween;

namespace Runtime.Common.Services.Audio
{
	public class AudioService : IAudioService
	{
		private EventInstance _currentAmbient;
		private EventReference _currentAmbientRef;

		// Храним список инстансов для каждого события, чтобы можно было их остановить
		private readonly Dictionary<Guid, List<EventInstance>> _activeInstances = new();

		public async void PlaySfx(EventReference eventRef, Vector3 position = default, Action<EventReference> onEnd = null, float volume = 1f)
		{
			if (eventRef.IsNull) return;

			// Если колбэк не нужен, используем самый быстрый путь
			if (onEnd == null)
			{
				RuntimeManager.PlayOneShot(eventRef, position);
				return;
			}

			// Если нужен onEnd, создаем управляемый инстанс
			var instance = CreateTrackedInstance(eventRef, position, volume);

			await WaitForEnd(instance);
			
			onEnd?.Invoke(eventRef);
		}

		public void PlayAmbient(EventReference eventRef, float fadeDuration = 1f, float targetVolume = 1f)
		{
			if (eventRef.IsNull) return;

			if (_currentAmbient.isValid())
			{
				if (_currentAmbientRef.Guid == eventRef.Guid) return;
				StopAmbient(fadeDuration);
			}

			_currentAmbientRef = eventRef;
			_currentAmbient = RuntimeManager.CreateInstance(eventRef);
			_currentAmbient.setVolume(0f);
			_currentAmbient.start();

			Tween.Custom(0f, targetVolume, duration: fadeDuration, onValueChange: val =>
			{
				if (_currentAmbient.isValid()) _currentAmbient.setVolume(val);
			});
		}

		public void StopAmbient(float fadeDuration = 1f)
		{
			if (!_currentAmbient.isValid()) return;

			var instance = _currentAmbient;
			_currentAmbient = default;
			_currentAmbientRef = default;

			instance.getVolume(out float startVolume);

			Tween.Custom(startVolume, 0f, duration: fadeDuration, onValueChange: val =>
				{
					if (instance.isValid()) instance.setVolume(val);
				})
				.OnComplete(() =>
				{
					if (instance.isValid())
					{
						instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
						instance.release();
					}
				});
		}

		public void StopPlaying(EventReference eventRef, bool immediate = false)
		{
			if (eventRef.IsNull) return;

			var mode = immediate ? FMOD.Studio.STOP_MODE.IMMEDIATE : FMOD.Studio.STOP_MODE.ALLOWFADEOUT;
			var guid = eventRef.Guid;

			// 1. Проверяем эмбиент
			if (_currentAmbientRef.Guid == guid)
			{
				StopAmbient(immediate ? 0f : 1f);
			}

			// 2. Останавливаем все SFX инстансы этого типа
			if (_activeInstances.TryGetValue(guid, out var instances))
			{
				for (int i = instances.Count - 1; i >= 0; i--)
				{
					var inst = instances[i];
					if (inst.isValid())
					{
						inst.stop(mode);
						inst.release();
					}
				}

				instances.Clear();
			}
		}

		private EventInstance CreateTrackedInstance(EventReference eventRef, Vector3 position, float volume)
		{
			var instance = RuntimeManager.CreateInstance(eventRef);
			instance.set3DAttributes(position.To3DAttributes());
			instance.setVolume(volume);
			instance.start();

			// Важно: в FMOD release() не стопает звук, а помечает его на удаление после завершения
			instance.release();

			var guid = eventRef.Guid;
			if (!_activeInstances.ContainsKey(guid))
				_activeInstances[guid] = new List<EventInstance>();

			_activeInstances[guid].Add(instance);

			// Запускаем очистку списка после завершения звука
			RemoveFromListOnEnd(instance, guid);

			return instance;
		}

		private async void RemoveFromListOnEnd(EventInstance instance, Guid guid)
		{
			await WaitForEnd(instance);
			if (_activeInstances.TryGetValue(guid, out var list))
			{
				list.Remove(instance);
			}
		}

		private async Task WaitForEnd(EventInstance instance)
		{
			if (!instance.isValid()) return;

			PLAYBACK_STATE state;
			instance.getPlaybackState(out state);

			while (state != PLAYBACK_STATE.STOPPED && instance.isValid())
			{
				await Task.Yield();
				if (instance.isValid())
					instance.getPlaybackState(out state);
				else
					break;
			}
		}
	}
}