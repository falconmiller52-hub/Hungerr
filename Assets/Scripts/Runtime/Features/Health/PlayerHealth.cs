using Runtime.UI;
using UnityEngine;

namespace Runtime.Features.Health
{
	public class PlayerHealth : MonoBehaviour, IDamageable
	{
		[SerializeField] private CounterUI _counterUI;
		[SerializeField] private float _maxHealth;
		
		private float _currentHealth;

		private float CurrentHealth
		{
			get => _currentHealth;
			set
			{
				_currentHealth = Mathf.Clamp(value, 0, _maxHealth);
			}
		}

		private void Start()
		{
			_currentHealth = _maxHealth;
			
			_counterUI.UpdateUI(_currentHealth, _maxHealth);
		}


		public void ApplyDamage(int value)
		{
			CurrentHealth -= value;
			
			_counterUI.UpdateUI(_currentHealth, _maxHealth);
		}
	}
}
