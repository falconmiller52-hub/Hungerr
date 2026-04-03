using PrimeTween;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Features.Health
{
	public class PlayerHealth : MonoBehaviour, IDamageable
	{
		[SerializeField] private CounterUI _counterUI;
		[SerializeField] private float _maxHealth;
		
		[Header("Shake Settings")]
		[SerializeField] private Transform _shakeCameraTransform;
		[SerializeField, Tooltip("Длительность тряски")] private float _shakeDuration = 0.3f;
		[SerializeField, Tooltip("Сила тряски по осям (вращение)")] private Vector3 _shakeStrength = new Vector3(5f, 5f, 2f); 
		[SerializeField, Tooltip("Насколько часто будет дергаться камера")] private int _vibrato = 10; 
		
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
			
			ShakeCameraOnDamage();
		}
		
		private void ShakeCameraOnDamage()
		{
			if (_shakeCameraTransform == null) 
				return;
			
			Tween.ShakeLocalRotation(_shakeCameraTransform, _shakeStrength, _shakeDuration, _vibrato);

		}
	}
}
