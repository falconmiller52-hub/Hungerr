using NaughtyAttributes;
using Runtime.Common.Services.Input;
using Runtime.Features.Outline;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Interactions
{
	public class PlayerScope : MonoBehaviour
	{
		//Переменные инспектора
		[SerializeField, Label("Player Camera")]
		private Camera _playerCamera;

		[Space, SerializeField, Label("Raycasting Length")]
		private float _rayLength;

		//Внутренние переменные
		private RaycastHit _rayHit;
		private GameObject _interactableObject;
		private IOutline _currentOutlineObject;
		private IInputHandler _inputHandler;

		//Кэшированные переменные


		[Inject]
		private void Construct(IInputHandler inputHandler)
		{
			_inputHandler = inputHandler;
		}

		private void OnEnable()
		{
			if (_inputHandler == null)
			{
				Debug.LogError("PlayerScope::OnEnable() No Input Handler was assigned");
				return;
			}

			_inputHandler.InteractPerformed += Interact;
		}

		private void OnDisable()
		{
			if (_inputHandler == null)
			{
				Debug.LogError("PlayerScope::OnDisable() No Input Handler was assigned");
				return;
			}

			_inputHandler.InteractPerformed -= Interact;
		}

		private void Update()
		{
			var ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			Physics.Raycast(ray, out _rayHit, _rayLength);

			if (_rayHit.collider && _rayHit.collider.gameObject != _interactableObject && _rayHit.collider.gameObject.TryGetComponent(out IOutline oi))
			{
				_interactableObject = _rayHit.collider.gameObject;

				_currentOutlineObject = oi;
				_currentOutlineObject.Enable(25f);
			}
			else if ((!_rayHit.collider || _rayHit.collider && _rayHit.collider.gameObject != _interactableObject) && _interactableObject)
			{
				_currentOutlineObject.Disable(0f);

				_currentOutlineObject = null;
				_interactableObject = null;
			}
		}

		//Методы скрипта
		private void Interact()
		{
			var ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			Physics.Raycast(ray, out var rayHit, _rayLength);

			if (rayHit.collider && rayHit.collider.gameObject != _interactableObject && rayHit.collider.gameObject.TryGetComponent(out IInteractable interactable))
			{
				interactable.Interact();
			}
		}
	}
}