using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.UI;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Other
{
	public class PlayerFoodController : MonoBehaviour
	{
		[SerializeField] private CounterUI _counterUI;
		[SerializeField] private float _maxFood;
		[SerializeField] private float _foodDrainPerSecond;

		private EventBus _eventBus;
		private float _currentFood;
		private bool _isActiveFoodDrain;

		[Inject]
		private void Construct(EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		private float CurrentFood
		{
			get => _currentFood;
			set { _currentFood = Mathf.Clamp(value, 0, _maxFood); }
		}

		private void Start()
		{
			_currentFood = _maxFood;

			_counterUI.UpdateUI(_currentFood, _maxFood);
		}

		private void Update()
		{
			if (_isActiveFoodDrain)
				ApplyFoodDrain(_foodDrainPerSecond * Time.deltaTime);
		}

		public void ApplyFoodDrain(float value)
		{
			CurrentFood -= value;

			if (CurrentFood <= 0)
				_eventBus.Trigger(EGameOver.PlayerOnZeroFood);

			_counterUI.UpdateUI(_currentFood, _maxFood, type: "F0");
		}
		
		public void ApplyFoodIncrease(float value)
		{
			CurrentFood += value;
			
			_counterUI.UpdateUI(_currentFood, _maxFood, type: "F0");
		}

		public void SetActiveFoodDrain(bool active)
		{
			_isActiveFoodDrain = active;
		}
	}
}