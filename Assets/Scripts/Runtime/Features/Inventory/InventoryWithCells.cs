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
            if (item == null) return false;
        
            // Проверка границ
            if (topLeftPosition.x + item._data.width > width ||
                topLeftPosition.y + item._data.height > height)
                return false;
        
            // Проверка занятости ячеек (проверяем все слоты по площади предмета - если свободно или можно стакать то чилл)
            for (int y = 0; y < item._data.height; y++)
            {
                for (int x = 0; x < item._data.width; x++)
                {
                    Vector2Int pos = new Vector2Int(
                        topLeftPosition.x + x, 
                        topLeftPosition.y + y
                    );
                
                    var slot = GetSlot(pos);
                    if (!slot.IsEmpty && !slot.CanStackWith(item))
                        return false;
                }
            }
        
            return true;
        }
    
        public bool AddItem(InventoryItem item, Vector2Int? position = null)
        {
            if (item == null) 
                return false;
        
            // Если указан конкретный позицию, пробуем туда
            if (position.HasValue)
            {
                if (CanPlaceItem(item, position.Value))
                {
                    PlaceItem(item, position.Value);
                    items.Add(item);
                    return true;
                }
                return false;
            }
        
            // Ищем подходящее место
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (CanPlaceItem(item, pos))
                    {
                        PlaceItem(item, pos);
                        items.Add(item);
                        return true;
                    }
                }
            }
        
            return false;
        }
    
        private void PlaceItem(InventoryItem item, Vector2Int topLeft)
        {
            for (int y = 0; y < item._data.height; y++)
            {
                for (int x = 0; x < item._data.width; x++)
                {
                    Vector2Int pos = new Vector2Int(topLeft.x + x, topLeft.y + y);
                    var slot = GetSlot(pos);
                
                    if (slot.IsEmpty)
                    {
                        slot.item = item;
                    }
                    else if (slot.CanStackWith(item)) // а нужна ли вторая проверка? он проверяет это же в CanPlaceItem
                    {
                        slot.TryAddItem(item);
                    }
                }
            }
        }
    
        public InventoryItem RemoveItem(InventoryItem item, int amount = 1)
        {
            if (item == null) return null;
        
            int removed = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    var slot = GetSlot(pos);
                
                    if (slot.item == item)
                    {
                        removed += slot.RemoveItem(amount - removed);
                        if (removed >= amount)
                            break;
                    }
                }
                if (removed >= amount) break;
            }
        
            if (removed > 0)
            {
                items.Remove(item);
                var newItem = new InventoryItem(item._data, item._amount - removed);
                if (newItem._amount > 0 && AddItem(newItem))
                    return newItem;
            }
        
            return null;
        }
    
        public bool TryTakeItem(InventoryItemData data, out InventoryItem taken, int amount = 1)
        {
            taken = null;
        
            foreach (var slot in slots.Values)
            {
                if (slot.item != null && slot.item._data == data && slot.item._amount >= amount)
                {
                    taken = new InventoryItem(data, amount);
                    slot.RemoveItem(amount);
                    return true;
                }
            }
        
            return false;
        }
    
        public IEnumerable<InventoryItem> GetAllItems() => items;
    
        public void MoveItem(InventoryItem item, Vector2Int newPosition)
        {
            // Удаление из старой позиции
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    var slot = GetSlot(pos);
                    if (slot.item == item)
                    {
                        slot.item = null;
                    }
                }
            }
        
            // Помещение в новую позицию
            PlaceItem(item, newPosition);
        }
    }
}