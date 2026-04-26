using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Runtime.Common.Services.Audio.Sound;
using UnityEngine;
using Zenject;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Runtime.Common.Services.Audio.Ost
{
	// TODO: как то переходы работают не так, нужно послушать и подумать как сделать правильнее
	public class OstService : MonoBehaviour
	{
		private SoundService _soundService;

		private EventInstance _currentOstInstance;
		private EventInstance _newOstInstance;

		private Coroutine _ostFadeRoutine;

		[Inject]
		private void Construct(SoundService soundService)
		{
			_soundService = soundService;
		}

		public void StartOst(EventReference eventReference)
		{
			_currentOstInstance.getPlaybackState(out PLAYBACK_STATE state);

			if (state == PLAYBACK_STATE.PLAYING)
			{
				if (_ostFadeRoutine == null)
				{
					_ostFadeRoutine = StartCoroutine(OstFadeRoutine(eventReference));
					//_currentOstInstance.stop(STOP_MODE.ALLOWFADEOUT);		
				}
				else
					Debug.LogError("Ost попытался запуститься пока другой ost тоже пытался запуститься");

				return;
			}

			_currentOstInstance = _soundService.PlaySound(eventReference);
		}

		private IEnumerator OstFadeRoutine(EventReference eventReference)
		{
			_currentOstInstance.stop(STOP_MODE.ALLOWFADEOUT);
			Debug.Log("Старый остановлен, запускаем новый");

			var newOstInstance = _soundService.PlaySound(eventReference);
			Debug.Log("Новый запущен");

			PLAYBACK_STATE state;

			do
			{
				_currentOstInstance.getPlaybackState(out state);
				yield return null;
			} while (state != PLAYBACK_STATE.STOPPED);

			Debug.Log("Старый полностью остановился");

			_currentOstInstance.release();
			_currentOstInstance = newOstInstance;
			_ostFadeRoutine = null;
		}
	}
}