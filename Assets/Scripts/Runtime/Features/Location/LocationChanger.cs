using NaughtyAttributes;
using UnityEngine;

namespace Runtime.Features.Location
{
	/// <summary>
	/// Скрипт отвечающий за хендл смены позиции игрока, к нему обращаются все компоненты-триггеры
	/// </summary>
	public class LocationChanger : MonoBehaviour
	{
		private CharacterController _playerController;

		[SerializeField, ReadOnly] private bool _isProcessing;
		
		public void Init(CharacterController characterController)
		{
			_playerController = characterController;
		}

		/// <summary>
		/// Метод который телепортирует персонажа в определенную точку.
		/// </summary>
		/// <param name="targetPoint">Позиция телепорта</param>
		public void ChangeLocation(Transform targetPoint)
		{
			if (_isProcessing)
				return;

			if (_playerController == null)
			{
				Debug.LogError("LocationChanger::ChangeLocation() PlayerController is null");
				return;
			}

			LocationChangeRoutine(targetPoint);
		}

		private void LocationChangeRoutine(Transform target)
		{
			_isProcessing = true;
			_playerController.enabled = false;

			Teleport(target);
			
			_isProcessing = false;
			_playerController.enabled = true;
		}

		private void Teleport(Transform target)
		{
			_playerController.transform.SetPositionAndRotation(target.position, target.rotation);
		}
	}
}