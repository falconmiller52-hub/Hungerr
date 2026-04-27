using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Runtime.Common.Helpers;
using Runtime.Common.InspectorFeatures.ButtonEditor;
using Runtime.Features.Inventory;
using UnityEngine;

namespace Runtime.Common.Services.ItemsIdentifier
{
	/// <summary>
	/// Need to be installed in zenject 
	/// </summary>
	[CreateAssetMenu(fileName = nameof(ItemsIdentifierSO), menuName = "Inventory/Items Identifier")]
	public class ItemsIdentifierSO : ScriptableObject, IButtonPressedHandler
	{
		[SerializeField] private string _path = "";

		[SerializeField, ReadOnly] private List<InventoryItemData> _items;

		private Dictionary<int, InventoryItemData> _itemsMap;

		public void OnButtonPressed()
		{
#if UNITY_EDITOR
			LoadItems();
#endif
		}

		public InventoryItemData GetItemDataById(int id)
		{
			Init();

			if (_itemsMap.TryGetValue(id, out var item))
				return item;

			Debug.LogError($"{nameof(ItemsIdentifierSO)}::{nameof(GetItemDataById)} no item with id: {id}");
			return null;
		}

		private void Init()
		{
			if (_items == null)
				LoadItems();

			_itemsMap ??= _items.ToDictionary(item => item.Id, item => item);
		}

		private void LoadItems()
		{
			_items = EditorFinder.Instance.GetAssetsFromFolder<InventoryItemData>(_path).ToList();
		}
	}
}