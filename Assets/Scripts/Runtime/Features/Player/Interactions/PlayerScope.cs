using FMODUnity;
using NaughtyAttributes;
using Runtime.Common.Services.Audio;
using Runtime.Common.Services.Input;
using Runtime.Common.Services.Pause;
using Runtime.Features.Interactable;
using Runtime.Features.Inventory;
using Runtime.Features.Inventory.WorldItem;
using Runtime.Features.Outline;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Player.Interactions
{
	public class PlayerScope : MonoBehaviour, IPausable
	{
		//Переменные инспектора
		[SerializeField, Label("Player Camera")]
		private UnityEngine.Camera _playerCamera;

		[Space, SerializeField, Label("Raycasting Length")]
		private float _rayLength;

		[Space, SerializeField, Label("Player Inventory")]
		private PlayerInventory _playerInventory;

		[SerializeField] private EventReference _pickUpSound;

		//Внутренние переменные
		private RaycastHit _rayHit;
		private GameObject _interactableObject;
		private bool _isCanInteract = true;

		//Кэшированные переменные
		private IOutline _currentOutlineObject;
		private IHoverable _currentHoveredObject;
		private IInputHandler _inputHandler;
		private IPauseController _pauseController;
		private IAudioService _audioService;

		[Inject]
		private void Construct(IInputHandler inputHandler, IPauseController pauseController, IAudioService audioService)
		{
			_inputHandler = inputHandler;
			_pauseController = pauseController;
			_audioService = audioService;
		}

		private void OnEnable()
		{
			if (_inputHandler == null)
			{
				Debug.LogError("PlayerScope::OnEnable() No Input Handler was assigned");
				return;
			}

			_inputHandler.InteractPerformed += Interact;

			if (_pauseController == null)
			{
				Debug.LogError("PlayerScope::OnEnable() No Pause Controller was assigned");
				return;
			}

			_pauseController.Add(this);
		}

		private void OnDisable()
		{
			if (_inputHandler == null)
			{
				Debug.LogError("PlayerScope::OnDisable() No Input Handler was assigned");
				return;
			}

			_inputHandler.InteractPerformed -= Interact;

			if (_pauseController == null)
			{
				Debug.LogError("PlayerScope::OnDisable() No Pause Controller was assigned");
				return;
			}

			_pauseController.Remove(this);
		}

		private void Update()
		{
			// 1. Делаем рейкаст
			var ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			bool hitSomething = Physics.Raycast(ray, out _rayHit, _rayLength);

			// Получаем объект, если он есть и он НЕ находится в процессе удаления
			GameObject hitObject = (hitSomething && _rayHit.collider != null) ? _rayHit.collider.gameObject : null;

			// 2. Если мы потеряли объект или переключились на другой
			if (hitObject != _interactableObject)
			{
				ClearCurrentTarget();
			}

			// 3. Если мы нашли новый объект и на нем нет метки удаления
			if (hitObject != null && _interactableObject == null)
			{
				// Проверка: вдруг объект уже помечен как уничтожаемый (через ваш кастомный код)
				if (hitObject.activeInHierarchy)
				{
					if (hitObject.TryGetComponent(out IHoverable hoverable))
					{
						_currentHoveredObject = hoverable;
						_currentHoveredObject.HoverEnter();
					}

					if (hitObject.TryGetComponent(out IOutline oi))
					{
						_currentOutlineObject = oi;
						_currentOutlineObject.Enable(25f);
					}

					_interactableObject = hitObject;
				}
			}
		}

		private void Interact()
		{
			if (!_isCanInteract || _interactableObject == null) return;

			// Сначала сохраняем ссылку
			GameObject target = _interactableObject;

			if (target.TryGetComponent(out IInteractable interactable))
				interactable.Interact();

			if (target.TryGetComponent(out WorldItem worldItem))
			{
				if (_playerInventory.AddItem(worldItem.Instance))
				{
					_audioService.PlaySfx(_pickUpSound);
					// ВАЖНО: Сначала полностью очищаем всё состояние интерфейса
					ClearCurrentTarget();

					// Отключаем объект немедленно, чтобы Raycast его больше не видел
					target.SetActive(false);

					// Удаляем в конце кадра
					Destroy(target);
				}
			}
		}

		private void ClearCurrentTarget()
		{
			if (_interactableObject != null)
			{
				_currentOutlineObject?.Disable(0f);
				_currentHoveredObject?.HoverExit();
			}

			_currentOutlineObject = null;
			_currentHoveredObject = null;
			_interactableObject = null;
		}

		//Методы скрипта

		public void Stop()
			=> SetDisableInteract();

		public void Resume()
			=> SetEnableInteract();

		private void SetEnableInteract()
			=> _isCanInteract = true;

		private void SetDisableInteract()
			=> _isCanInteract = false;
	}
}