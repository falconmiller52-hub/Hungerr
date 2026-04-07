// InventoryUIView.cs

using Runtime.Features.Inventory;
using UnityEngine;
using Zenject;

public class InventoryUIView : MonoBehaviour
{
	[SerializeField] private Transform _slotParent;
	[SerializeField] private GameObject _slotPrefab;
	[SerializeField] private int _gridWidth = 8;
	[SerializeField] private int _gridHeight = 4;
	[SerializeField] private PlayerInventory _playerInventory;
	private InventorySlotView[,] _slotViews;
    
	private void Start()
	{
		CreateSlots();
		Refresh();
        
		_playerInventory.OnInventoryChanged += Refresh;
	}
    
	private void CreateSlots()
	{
		_slotViews = new InventorySlotView[_gridWidth, _gridHeight];
        
		for (int y = 0; y < _gridHeight; y++)
		{
			for (int x = 0; x < _gridWidth; x++)
			{
				var slotGO = Instantiate(_slotPrefab, _slotParent);
				var slotView = slotGO.GetComponent<InventorySlotView>();
				slotView.Initialize(new Vector2Int(x, y));
				_slotViews[x, y] = slotView;
			}
		}
	}
    
	public void Refresh()
	{
		var inventory = _playerInventory.GetInventory();
        
		for (int y = 0; y < _gridHeight; y++)
		{
			for (int x = 0; x < _gridWidth; x++)
			{
				Vector2Int pos = new Vector2Int(x, y);
				var slot = inventory.GetSlot(pos);
				_slotViews[x, y].SetItem(slot.item);
			}
		}
	}
    
	private void OnDestroy()
	{
		_playerInventory.OnInventoryChanged -= Refresh;
	}
}