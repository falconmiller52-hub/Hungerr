using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Runtime.Common.Extensions;
using Runtime.Common.Services.Audio.Sound;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Runtime.Features.Enemy.Domovoi.Patterns
{
	public class PlayScarySoundPattern : DomovoiPattern
	{
		[SerializeField] private EventReference _proximitySoundEvent;
		[SerializeField] private Transform _centerOfProximitySound;
		
		[SerializeField] private EventReference _randomSoundEvent;
		[SerializeField] private List<Transform> _randomSoundPoints;
		[SerializeField] private float _minDelay = 5f;
		[SerializeField] private float _maxDelay = 15f;

		private ISoundService _soundService;
		private Coroutine _loopRoutine;
		Transform _pointToStartRandomSound;

		[Inject]
		private void Construct(ISoundService soundService)
		{
			_soundService = soundService;
		}

		private void Start()
		{
			_pointToStartRandomSound = transform;
			
			if (!_randomSoundPoints.Contains(_pointToStartRandomSound))
				_randomSoundPoints.Add(_pointToStartRandomSound);
		}

		private void OnDisable()
		{
			if (_loopRoutine != null)
			{
				StopCoroutine(_loopRoutine);
				_loopRoutine = null;
			}
		}

		public override void Trigger()
		{
			base.Trigger();

			if (_loopRoutine != null)
				StopCoroutine(_loopRoutine);

			// Запускаем бесконечный цикл проигрывания
			_loopRoutine = StartCoroutine(PlayRandomSoundsRoutine());
			
			_soundService.PlayOneShotAt(_proximitySoundEvent, _centerOfProximitySound.position);
		}

		private IEnumerator PlayRandomSoundsRoutine()
		{
			while (true)
			{
				if (_randomSoundPoints.Count > 0)
					_pointToStartRandomSound = _randomSoundPoints.RandomExcept(_pointToStartRandomSound);
				
				_soundService.PlayOneShotAt(_randomSoundEvent, _pointToStartRandomSound.position);

				float randomDelay = Random.Range(_minDelay, _maxDelay);

				yield return new WaitForSeconds(randomDelay);
			}
		}

		public override void Clear()
		{
			if (_loopRoutine != null)
			{
				StopCoroutine(_loopRoutine);
				_loopRoutine = null;
			}
		}
	}
}