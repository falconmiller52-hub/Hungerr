using UnityEngine;

namespace Runtime.Features.Inventory
{
	[CreateAssetMenu(fileName = "ItemData")]
	public class InventoryItemData : ScriptableObject
	{
		[Header("Basic Info")]
		public string itemName;
		public Sprite icon;
		public string description;
    
		[Header("Stack Settings")]
		public int maxStackSize = 64;
    
		[Header("3D World")]
		public GameObject worldPrefab; // 3D префаб предмета в мире
		public GameObject PrefabForInventory; 
    
		
		[Header("Item Size in Inventory")]
		public int width = 1;  // ширина в ячейках
		public int height = 1; // высота в ячейках
    
		public bool IsStackable => maxStackSize > 1;
	}
}