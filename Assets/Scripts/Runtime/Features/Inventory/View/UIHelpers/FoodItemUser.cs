using Runtime.Common.Services.Audio.Sound;
using Runtime.Common.Services.Input;
using Runtime.Features.Inventory.View.EntryPoint;
using Runtime.Features.Player.Other;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Inventory.View.UIHelpers
{
	public class FoodItemUser : MonoBehaviour
	{
		[Header("References")] 
		[SerializeField] private PlayerFoodController _playerFoodController;
		[SerializeField] private PlayerInventory _playerInventory;
		
		[Header("Settings")] 
		[SerializeField] private Inventory3DView _view;
		[SerializeField] private Camera _mainCamera;
		[SerializeField] private LayerMask _gridLayer;

		private IInputHandler _inputHandler;

		[Inject]
		private void Construct(ISoundService soundService, IInputHandler inputHandler)
		{
			_inputHandler = inputHandler;
		}

		private void Start()
		{
			_inputHandler.InventoryUsePressed += OnInventoryUsePressed;
		}

		private void OnDisable()
		{
			_inputHandler.InventoryUsePressed -= OnInventoryUsePressed;
		}

		private void OnInventoryUsePressed()
		{
			Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
				
			if (Physics.Raycast(ray, out RaycastHit hit, 20f, _gridLayer))
			{
				Vector3 localPos = _view.GridAnchor.InverseTransformPoint(hit.point);
				Vector2Int hoveredCoords = LocalToGrid(localPos, _view);
				var slot = _view.Model.GetSlot(hoveredCoords);

				if (slot == null || slot.IsEmpty)
					return;

				if (slot.Item.Data is FoodInventoryItemData data)
				{
					_playerFoodController.ApplyFoodIncrease(data.Satiety);
					_playerInventory.RemoveItem(hoveredCoords);
				}
			}
		}

		private Vector2Int LocalToGrid(Vector3 localPos, Inventory3DView targetView)
		{
			int x = Mathf.FloorToInt(localPos.x / targetView.CellSize);
			int y = Mathf.FloorToInt(-localPos.z / targetView.CellSize);

			x = Mathf.Clamp(x, 0, targetView.Model.Width - 1);
			y = Mathf.Clamp(y, 0, targetView.Model.Height - 1);

			return new Vector2Int(x, y);
		}
	}
}