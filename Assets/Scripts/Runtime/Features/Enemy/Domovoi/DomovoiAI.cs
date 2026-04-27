using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Runtime.Common.Enums;
using Runtime.Common.Extensions;
using Runtime.Common.Services.EventBus;
using Runtime.Features.Enemy.Domovoi.Patterns;
using Runtime.Features.Inventory;
using Runtime.Features.Inventory.View.EntryPoint;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Enemy.Domovoi
{
	public class DomovoiAI : MonoBehaviour
	{
		[SerializeField] private StorageInventory _storage; // хранилище с едой
		[SerializeField] private DomovoiLevelData[] _domovoiLevelData;

		[SerializeField, ReadOnly] private int _satiety;
		[SerializeField, ReadOnly] private DomovoiLevelData _currentLevelData;
		private EventBus _eventBus;
		private InventoryWithCells _inventory;
		private int _notFedDaysCount;
		private bool _needToTriggerDontFeed;
		private EDomovoiSatietyLevel _satietyLevel;
		private DomovoiPattern _currentDomovoiPattern;

		[Inject]
		private void Construct(EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		private void Start()
		{
			// если нет сохранений состояния
			if (_domovoiLevelData == null || _domovoiLevelData.Length == 0)
			{
				Debug.LogError("DomovoiAI::Start() Domovoi level data is null");
				return;
			}

			if (_eventBus == null)
			{
				Debug.LogError("DomovoiAI::Start() EventBus is null");
				return;
			}

			_currentLevelData = _domovoiLevelData[0];
			_satiety = _currentLevelData.MaxSatiety;
			_notFedDaysCount = 0;

			_inventory = _storage.GetInventory();
		}

		/// <summary>
		/// метод высчета логики для домового днем
		/// </summary>
		/// <returns>bool as NeedToTriggerDontFeed | EDomovoiSatietyLevel as level of satiety</returns>
		public (bool, EDomovoiSatietyLevel) StartDayPhaseHandler()
		{
			// кидаем ивент в зависимости от уровня сытости, если мало (по текущему уровню) то будет больно
			if (_satiety < _currentLevelData.SatietyTreshholdForActivation)
				_satietyLevel = EDomovoiSatietyLevel.Critical;
			else
				_satietyLevel = EDomovoiSatietyLevel.Normal;
			
			// выбираем рандомный паттерн поведения на сейчас если уровень сытости - критический
			if (_satietyLevel == EDomovoiSatietyLevel.Critical)
			{
				_currentDomovoiPattern = _currentLevelData.Patterns.Random();
				_currentDomovoiPattern.Trigger();
			}

			if (_needToTriggerDontFeed)
			{
				// ставим false, высчитываем каждую ночь и сбрасываем каждый день
				_needToTriggerDontFeed = false;
				return (true, _satietyLevel);
			}

			return (false, _satietyLevel);
		}

		/// <summary>
		/// высчет логики и обновление голода домового ночью
		/// </summary>
		/// <param name="currentDay">int текущего дня</param>
		public void StartNightPhaseHandler(int currentDay)
		{
			// пытаемся очистить паттерн домового если есть возможность
			_currentDomovoiPattern.Clear();
			_currentDomovoiPattern = null;
			
			if (_inventory == null)
				_inventory = _storage.GetInventory();

			// проверяем день и переходим на некст уровень если надо
			foreach (DomovoiLevelData levelData in _domovoiLevelData)
			{
				if (currentDay >= levelData.MinDayForLevel && levelData != _currentLevelData)
					_currentLevelData = levelData;
			}

			UpdateSatietyStatus();

			_inventory.RemoveAllItemsByType<FoodInventoryItemData>();
		}

		private void UpdateSatietyStatus()
		{
			List<FoodInventoryItemData> foodItems = _inventory.GetItems<FoodInventoryItemData>();
			int totalSatietyFromInventory = foodItems.Sum(foodItem => foodItem.Satiety); // считаем всю сытость от еды в сундуке

			int delta = Math.Max(totalSatietyFromInventory, 0) - Math.Max(_currentLevelData.DailySatietyLoss, 0);
			_satiety = Math.Max(_satiety + delta, 0);

			// если нет еды то увеличиваем счтечик дней без еды для Домового, иначе сбрасываем его
			if (totalSatietyFromInventory <= 0)
			{
				_notFedDaysCount++;

				if (_notFedDaysCount >= _currentLevelData.NotFeededDaysAvailableBeforePunishment)
				{
					_needToTriggerDontFeed = true;
					_notFedDaysCount = 0;
				}
			}
			else
			{
				_notFedDaysCount = 0;
			}
		}
	}
}