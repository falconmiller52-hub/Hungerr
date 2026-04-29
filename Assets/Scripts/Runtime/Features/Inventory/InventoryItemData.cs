using System;
using FMODUnity;
using UnityEngine;

namespace Runtime.Features.Inventory
{
	[Serializable]
	[CreateAssetMenu(fileName = "ItemData",  menuName = "Inventory/Item Data")]
	public class InventoryItemData : ScriptableObject
	{
		[field: SerializeField] public int Id { get; private set; }
		
		[field: Header("Basic Info")]
		[field: SerializeField] public string ItemName { get; private set; } = String.Empty;
		[field: SerializeField] public string Description { get; private set; } = String.Empty;
    
		[field: Header("Stack Settings")]
		[field: SerializeField] public int MaxStackSize { get; private set; } = 64;
    
		[field: Header("3D World")]
		[field: SerializeField] public GameObject WorldPrefab { get; private set; } 
		[field: SerializeField] public GameObject PrefabForInventory { get; private set; }
		
		[field: Header("Item Size in Inventory")]
		[field: SerializeField] public int Width { get; private set; } = 1;  // ширина в ячейках
		[field: SerializeField] public int Height { get; private set; } = 1; // высота в ячейках
		
		[field: Header("Action Sounds in Inventory")]
		[field: SerializeField] public EventReference GrabItemInInventorySound { get; private set; }
		[field: SerializeField] public EventReference PutItemInInventorySound { get; private set; }
		
		public bool IsStackable => MaxStackSize > 1;
	}
}