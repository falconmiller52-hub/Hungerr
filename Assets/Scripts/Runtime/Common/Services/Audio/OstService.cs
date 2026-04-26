using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Zenject;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Runtime.Common.Services.Audio
{
	public class OstService : MonoBehaviour
	{
		private AudioService _audioService;

		private EventInstance _currentOstInstance;
		private EventInstance _newOstInstance;

		private Coroutine _ostFadeRoutine;

		[Inject]
		private void Construct(AudioService audioService)
		{
			_audioService = audioService;
		}

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

			_currentOstInstance = _audioService.PlaySound(eventReference);
		}

		private IEnumerator OstFadeRoutine(EventReference eventReference)
		{
			_currentOstInstance.stop(STOP_MODE.ALLOWFADEOUT);
			_newOstInstance = _audioService.PlaySound(eventReference);

			PLAYBACK_STATE state;

			do
			{
				_currentOstInstance.getPlaybackState(out state);
				yield return null;
			} while (state != PLAYBACK_STATE.STOPPED);

			_currentOstInstance.release();

			_currentOstInstance = _newOstInstance;

			_ostFadeRoutine = null;
		}
	}
}