// CellInventory.cs

using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Features.Inventory
{
	public class InventoryWithCells
	{
		private readonly int width;
		private readonly int height;
		private readonly Dictionary<Vector2Int, InventorySlot> slots;
		private readonly List<InventoryItem> items;

		public int Width => width;
		public int Height => height;
		public int TotalSlots => width * height;

		public Dictionary<Vector2Int, InventorySlot> Slots => slots;

		public InventoryWithCells(int width, int height)
		{
			this.width = width;
			this.height = height;
			slots = new Dictionary<Vector2Int, InventorySlot>();
			items = new List<InventoryItem>();

			InitializeSlots();
		}

		private void InitializeSlots()
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Vector2Int pos = new Vector2Int(x, y);
					slots[pos] = new InventorySlot(pos);
				}
			}
		}

		public InventorySlot GetSlot(Vector2Int pos)
		{
			slots.TryGetValue(pos, out var slot);
			return slot;
		}

		public bool CanPlaceItem(InventoryItem item, Vector2Int topLeftPosition)
		{
			if (item == null)
				return false;

			// 1. Проверка границ
			if (topLeftPosition.x + item._data.width > width ||
			    topLeftPosition.y + item._data.height > height)
				return false;

			int totalCells = item._data.width * item._data.height;
			int emptyCells = 0;
			int stackableCells = 0;

			for (int y = 0; y < item._data.height; y++)
			{
				for (int x = 0; x < item._data.width; x++)
				{
					var slot = GetSlot(new Vector2Int(topLeftPosition.x + x, topLeftPosition.y + y));

					if (slot.IsEmpty)
					{
						emptyCells++;
					}
					else if (slot.CanStackWith(item))
					{
						stackableCells++;
					}
					else
					{
						// Если хоть в одном слоте чужой предмет, который не стакается то сразу отмена
						return false;
					}
				}
			}

			// ЛОГИКА: 
			// Либо все ячейки пустые (размещение на свободное место)
			// Либо все ячейки подходят для стака (полное наложение на другой предмет)
			return emptyCells == totalCells || stackableCells == totalCells;
		}

		public bool AddItem(InventoryItem item, Vector2Int? position = null)
		{
			if (item == null)
				return false;

			// Если указан конкретный позицию, пробуем туда (может быть занято таким же предметом или быть пустым)
			if (position.HasValue)
			{
				if (CanPlaceItem(item, position.Value))
				{
					var slot = GetSlot(position.Value);
					int id;

					if (!slot.IsEmpty && slot.Id != -1)
						id = slot.Id;
					else
						id = Random.Range(0, 1000); // TODO: хуйня, переделать

					PlaceItem(item, position.Value, id);
					items.Add(item);
					return true;
				}

				return false;
			}

			// Ищем первое попавшееся подходящее место (может быть занято таким же преметом или быть пустым)
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Vector2Int pos = new Vector2Int(x, y);
					if (CanPlaceItem(item, pos))
					{
						var slot = GetSlot(pos);
						int id;

						if (!slot.IsEmpty && slot.Id != -1)
							id = slot.Id;
						else
							id = Random.Range(0, 1000); // TODO: хуйня, переделать

						PlaceItem(item, pos, id);
						items.Add(item);
						return true;
					}
				}
			}

			return false;
		}

		private void PlaceItem(InventoryItem item, Vector2Int topLeft, int id)
		{
			// в любом случае должно быть id != -1
			for (int y = 0; y < item._data.height; y++)
			{
				for (int x = 0; x < item._data.width; x++)
				{
					Vector2Int pos = new Vector2Int(topLeft.x + x, topLeft.y + y);
					var slot = GetSlot(pos);

					if (slot.IsEmpty)
					{
						slot.item = item;
						slot.Id = id;
					}
					else if (slot.CanStackWith(item)) // а нужна ли вторая проверка? он проверяет это же в CanPlaceItem
					{
						slot.TryAddItem(item, id);
					}
				}
			}
		}
	}
}