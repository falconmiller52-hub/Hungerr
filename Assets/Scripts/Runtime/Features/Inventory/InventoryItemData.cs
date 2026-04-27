using System;
using UnityEngine;

namespace Runtime.Features.Inventory
{
	[Serializable]
	[CreateAssetMenu(fileName = "ItemData",  menuName = "Inventory/Item Data")]
	public class InventoryItemData : ScriptableObject
	{
		[field: SerializeField] public int Id { get; private set; }
		
		[Header("Basic Info")]
		[field: SerializeField] public string ItemName { get; private set; } = String.Empty;
		[field: SerializeField] public string Description { get; private set; } = String.Empty;
    
		[Header("Stack Settings")]
		[field: SerializeField] public int MaxStackSize { get; private set; } = 64;
    
		[Header("3D World")]
		[field: SerializeField] public GameObject WorldPrefab { get; private set; } 
		[field: SerializeField] public GameObject PrefabForInventory { get; private set; }
		
		[Header("Item Size in Inventory")]
		[field: SerializeField] public int Width { get; private set; } = 1;  // ширина в ячейках
		[field: SerializeField] public int Height { get; private set; } = 1; // высота в ячейках
    
		public bool IsStackable => MaxStackSize > 1;
	}
}