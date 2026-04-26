using Runtime.Common.Services.EventBus;
using Runtime.Features.Location;
using UnityEngine;
using Zenject;
using Runtime.Features.Health;
using Random = UnityEngine.Random;

namespace Runtime.Features.Supervision
{
	public class SupervisionController : MonoBehaviour
	{
		[Header("Spawn Points")] 
		[SerializeField] private Transform _homeSpawnPoint;
		[SerializeField] private Transform _domovoiRoomSpawnPoint;

		[Header("Settings")] 
		[SerializeField] private float _baseDeathChance = 0.05f; // 5%
		[SerializeField] private float _chanceIncrement = 0.1f; // +10% за каждый проеб
		[SerializeField] private int _damageByDomovoi = 10;
		[SerializeField] private GameObject _bloodPuddle;

		private EventBus _eventBus;
		private LocationChanger _locationChanger;
		private PlayerHealth _playerHealth;

		private int _lateNightsCount = 0;
		private int _starvingCount = 0;

		[Inject]
		private void Construct(EventBus eventBus, LocationChanger locationChanger)
		{
			_eventBus = eventBus;
			_locationChanger = locationChanger;
		}

		private void Start()
		{
			_playerHealth = FindAnyObjectByType<PlayerHealth>();
		}

		public void ClearAllPunishObjects()
		{
			_bloodPuddle.SetActive(false);
		}
		
		public void OnLateAtNight()
		{
			_lateNightsCount++;

			if (_lateNightsCount == 1)
			{
				// Первый раз: Мягкое наказание
				HandleFirstLateNight();
			}
			else
			{
				// Последующие разы
				HandleSeverePunishment(_lateNightsCount);
				CheckBadEnding(_lateNightsCount);
			}
		}

		public void OnDomovoiDontFed()
		{
			_starvingCount++;

			// За некормление всегда жесткое наказание
			HandleSeverePunishment(_starvingCount);

			if (_starvingCount > 1)
				CheckBadEnding(_starvingCount);
		}

		private void HandleFirstLateNight()
		{
			// Телепорт домой
			_locationChanger.ChangeLocation(_homeSpawnPoint, needCurtain: false);

			// Эффекты
			// PlayAlarmSound();
			// ShowStaticNoiseEffect(); // Помехи
			// PlayDomovoiGrowl();

			Debug.Log("First late night: Alarm, Noise, Growl.");
		}

		private void HandleSeverePunishment(int count)
		{
			// Телепорт в комнату Домового
			_locationChanger.ChangeLocation(_domovoiRoomSpawnPoint, needCurtain: false);

			// Понижение ХП 
			if (_playerHealth != null)
				_playerHealth.ApplyDamage(_damageByDomovoi);

			// Включаем лужу крови
			_bloodPuddle.SetActive(true);

			Debug.Log($"Severe punishment. Count: {count}. Player in blood puddle.");
		}

		private void CheckBadEnding(int failCount)
		{
			// Формула: шанс растет с каждым проебом (начиная со второго)
			// (failCount - 1) т.к. первый раз шанс 0 или минимальный по ТЗ
			float currentChance = _baseDeathChance + (_chanceIncrement * (failCount - 2));

			if (Random.value < currentChance)
				TriggerBadEnding();
		}

		private void TriggerBadEnding()
		{
			Debug.Log("BAD ENDING TRIGGERED");
		}
	}
}