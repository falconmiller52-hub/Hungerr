using System;
using UnityEngine;

namespace Runtime.Features.Inventory.WorldItem
{
	[RequireComponent(typeof(MeshRenderer))]
	public class WorldItem : MonoBehaviour, IHoverable
	{
		[SerializeField] private InventoryItemData _itemData;
		[SerializeField] private int _amount = 1;

		public event Action<WorldItem> WorldItemDestroyed;

		private InventoryItem _instance;

		private void Awake()
		{
			_instance = new InventoryItem(_itemData, _amount);
		}

		// метод для инициализации из кода, например если предмет с моба выпадет или с инвентаря
		public void Initialize(InventoryItemData data, int count = 1)
		{
			_itemData = data;
			_amount = count;
			_instance = new InventoryItem(data, count);

			if (data.WorldPrefab != null)
			{
				var prefab = Instantiate(data.WorldPrefab, transform);
				prefab.transform.localPosition = Vector3.zero;
			}
		}

		public void HoverEnter()
		{
			TooltipController.Instance?.ShowTooltip(this);
		}

		public void HoverExit()
		{
			TooltipController.Instance?.HideTooltip();
		}

		public InventoryItem GetItem()
		{
			return _instance;
		}

		public void DestroyWorldItem()
		{
			WorldItemDestroyed?.Invoke(this);
			Destroy(gameObject);
		}

		public string GetDisplayName() => _itemData.ItemName;

		public string GetDescription()
		{
			switch (_itemData)
			{
				case FoodInventoryItemData food:
					return $"Сытость: {food.Satiety}";
				default:
					return _itemData.Description;
			}
		}
	}
}