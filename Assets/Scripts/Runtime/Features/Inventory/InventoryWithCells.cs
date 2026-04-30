using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Features.Inventory
{
	public class InventoryWithCells
	{
		private readonly int _width;
		private readonly int _height;
		private readonly Dictionary<Vector2Int, InventorySlot> _slots;
		private readonly List<InventoryItem> _items;

		public int Width => _width;
		public int Height => _height;

		public Dictionary<Vector2Int, InventorySlot> Slots => _slots;

		public InventoryWithCells(int width, int height)
		{
			this._width = width;
			this._height = height;
			_slots = new Dictionary<Vector2Int, InventorySlot>();
			_items = new List<InventoryItem>();

			InitializeSlots();
		}

		private void InitializeSlots()
		{
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					Vector2Int pos = new Vector2Int(x, y);
					_slots[pos] = new InventorySlot(pos);
				}
			}
		}

		public InventorySlot GetSlot(Vector2Int pos)
		{
			_slots.TryGetValue(pos, out InventorySlot slot);
			return slot;
		}

		/// <summary>
		/// Получаем список всех Items по типу даты
		/// </summary>
		/// <typeparam name="T"> T - тип InventoryItemData</typeparam>
		/// <returns></returns>
		public List<InventoryItem> GetItems<T>() where T : InventoryItemData
		{
			List<InventoryItem> items = new List<InventoryItem>();
			
			foreach (var slot in Slots.Values)
			{
				if (!slot.IsEmpty && slot.Item.Data is T data)
					items.Add(slot.Item);
			}
			
			return items;
		}

		public bool CanPlaceItem(InventoryItem item, Vector2Int topLeftPosition)
		{
			if (item == null)
				return false;

			// 1. Проверка границ
			if (topLeftPosition.x + item.Data.Width > _width ||
			    topLeftPosition.y + item.Data.Height > _height)
				return false;

			int totalCells = item.Data.Width * item.Data.Height;
			int emptyCells = 0;
			int stackableCells = 0;

			for (int y = 0; y < item.Data.Height; y++)
			{
				for (int x = 0; x < item.Data.Width; x++)
				{
					InventorySlot slot = GetSlot(new Vector2Int(topLeftPosition.x + x, topLeftPosition.y + y));

					if (slot.IsEmpty)
						emptyCells++;
					else if (slot.CanStackWith(item))
						stackableCells++;
					else
						// Если хоть в одном слоте чужой предмет, который не стакается то сразу отмена
						return false;
					
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
					InventorySlot slot = GetSlot(position.Value);
					int id;

					if (!slot.IsEmpty && slot.Id != -1)
						id = slot.Id;
					else
						id = Random.Range(0, 1000); // TODO: хуйня, переделать

					PlaceItem(item, position.Value, id);
					_items.Add(item);
					return true;
				}

				return false;
			}

			// Ищем первое попавшееся подходящее место
			// (сначала проходим по инвентарю и ищем слоты с этим предметом чтоб стакнуть, если их нет, то ставим в первое свободное место)
			foreach (InventorySlot slot in _slots.Values)
			{
				if (slot.CanStackWith(item))
				{
					PlaceItem(item, slot.Position, slot.Id);
					_items.Add(item);
					return true;
				}
			}
			
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					Vector2Int pos = new Vector2Int(x, y);
					
					if (CanPlaceItem(item, pos))
					{
						InventorySlot slot = GetSlot(pos);
						int id;

						if (!slot.IsEmpty && slot.Id != -1)
							id = slot.Id;
						else
							id = Random.Range(0, 1000); // TODO: хуйня, переделать

						PlaceItem(item, pos, id);
						_items.Add(item);
						return true;
					}
				}
			}

			return false;
		}

		public bool RemoveItemByPosition(Vector2Int pos, int amount = -1)
		{
			InventorySlot slot = GetSlot(pos);
			IEnumerable<InventorySlot> neededSlots = _slots.Values.Where(x => x.Id == slot.Id);

			foreach (InventorySlot neededSlot in neededSlots)
			{
				if (amount < 1)
					neededSlot.RemoveItem(999); // типа удаляем весь айтем (калич)
				else
					neededSlot.RemoveItem(amount);
			}

			return true;
		}

		public void RemoveAllItemsByType<T>() where T : InventoryItemData
		{
			IEnumerable<InventorySlot> neededSlots = _slots.Values.Where(x => !x.IsEmpty && x.Item.Data is T);

			foreach (InventorySlot neededSlot in neededSlots)
			{
				neededSlot.RemoveItem(999); // типа удаляем весь айтем (калич)
			}
		}

		private void PlaceItem(InventoryItem item, Vector2Int topLeft, int id)
		{
			bool increaseApplied = false;
			
			// в любом случае должно быть id != -1
			for (int y = 0; y < item.Data.Height; y++)
			{
				for (int x = 0; x < item.Data.Width; x++)
				{
					Vector2Int pos = new Vector2Int(topLeft.x + x, topLeft.y + y);
					InventorySlot slot = GetSlot(pos);

					if (slot.IsEmpty)
					{
						slot.Item = item;
						slot.Id = id;
					}
					else if (slot.CanStackWith(item) && !increaseApplied)
					{
						// увеличиваем  amount только один раз так как все слоты имеют ссылку на один айтем
						slot.TryAddItem(item, id);
						increaseApplied = true;
					}
				}
			}
		}
	}
}