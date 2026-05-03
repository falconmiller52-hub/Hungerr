using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Runtime.Common.Services.Audio.Sound;
using UnityEngine;
using Zenject;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Runtime.Common.Services.Audio.Ost
{
	public class OstService : MonoBehaviour
	{
		private ISoundService _soundService;

		private EventInstance _currentOstInstance;
		private EventInstance _newOstInstance;

		private Coroutine _ostFadeRoutine;

		[Inject]
		private void Construct(ISoundService soundService)
		{
			_soundService = soundService;
		}
		
		/// <summary>
		/// Меняем Ost звук. Старый плавно затухает, новый плавно нарастает
		/// </summary>
		/// <param name="eventReference">Ost который нужно запустить</param>
		public void StartOst(EventReference eventReference)
		{
			_currentOstInstance.getPlaybackState(out PLAYBACK_STATE state);

			if (state == PLAYBACK_STATE.PLAYING)
			{
				if (_ostFadeRoutine == null)
					_ostFadeRoutine = StartCoroutine(OstFadeRoutine(eventReference));
				else
					Debug.LogError("Ost попытался запуститься пока другой ost тоже пытался запуститься");

				return;
			}

			_currentOstInstance = _soundService.PlaySound(eventReference);
		}

		private IEnumerator OstFadeRoutine(EventReference eventReference)
		{
			_currentOstInstance.stop(STOP_MODE.ALLOWFADEOUT);

			var newOstInstance = _soundService.PlaySound(eventReference);

			PLAYBACK_STATE state;

			do
			{
				_currentOstInstance.getPlaybackState(out state);
				yield return null;
			} while (state != PLAYBACK_STATE.STOPPED);

			_currentOstInstance.release();
			_currentOstInstance = newOstInstance;
			_ostFadeRoutine = null;
		}
	}
}