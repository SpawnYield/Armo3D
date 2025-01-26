using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.AddressableAssets;


[Serializable]
public struct Item
{
    public int ItemNameId;           // Индикатор для названия объекта (можно использовать для поиска)
    public int PrefabId;             // Индикатор для префаба
    public int ScriptId;             // Индикатор для выполнения скрипта (можно использовать для поиска)
    public int ImageId;
    public int ItemPrice;            // Цена объекта
    public int ItemCount;            // Текущее количество объекта
    public int MaxItemCount;         // Максимальное количество объекта

    public Item(int itemNameId, int prefabId, int scriptId,int imageId, int itemPrice, int itemCount, int maxItemCount)
    {
        ItemNameId = itemNameId;
        PrefabId = prefabId;  // Заменяем null на пустую строку
        ScriptId = scriptId;  // Заменяем null на пустую строку
        ImageId = imageId;  // Заменяем null на пустую строку
        ItemPrice = itemPrice;
        ItemCount = itemCount;
        MaxItemCount = maxItemCount;
    }
    public Item(Item item)
    {
        this = item;
    }
    
    public readonly bool IsEmpty => ItemNameId < 0;

    public static Item operator +(Item a, Item b)
    {
        if (a.ItemNameId == b.ItemNameId)  // Если названия совпадают, складываем количество
        {
            return new Item(a.ItemNameId, a.PrefabId, a.ScriptId,a.ImageId, a.ItemPrice, a.ItemCount + b.ItemCount, a.MaxItemCount);
        }
        return a;
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is Item item)
        {
            return ItemNameId == item.ItemNameId;
        }
        return false;
    }

    public override readonly int GetHashCode() => ItemNameId.GetHashCode();
}

public class Inventory : IDisposable
{
    public static NativeHashMap<int, Item> Items;
    public event Action<Item> OnItemAdded;
    public event Action<int> OnItemRemoved;
    public event Action<Item> ItemMax;
    public Inventory()
    {
        Items = new NativeHashMap<int, Item>(2048*12, Allocator.Persistent);
    }

    public void AddItem(Item item)
    {
        if (Items.ContainsKey(item.ItemNameId))
        {

            if (Items[item.ItemNameId].ItemCount + item.ItemCount >= item.MaxItemCount)
            {
                ItemMax?.Invoke(item);
                return;
            }
                
            Items[item.ItemNameId] = Items[item.ItemNameId] + item;
        }
        else
        {
            Items.Add(item.ItemNameId, item);
        }
        OnItemAdded?.Invoke(item); // Уведомляем о добавлении

    }

    public void AddItems(IEnumerable<Item> items)
    {
        foreach (var item in items)
        {
            AddItem(item);
        }
    }

    public Item ContainsItem(int id)
    {
        return Items.TryGetValue(id, out var item) ? item : new Item(-1,-1, -1, -1, -1, -1, -1);
    }
    public Item GetItem(int id)
    {
        return Items.TryGetValue(id, out var item) ? item : default;
    }
    public void RemoveItem(int id)
    {
        if (Items.Remove(id))
        {
            OnItemRemoved?.Invoke(id); // Уведомляем об удалении
        }
    }

    public void RemoveItems(IEnumerable<int> ids)
    {
        foreach (var id in ids)
        {
            RemoveItem(id);
        }
    }

    public List<Item> GetAllItems()
    {
        var allItems = new List<Item>();
        foreach (var originalItem in Items)
        {
            allItems.Add(originalItem.Value);
        }
        return allItems;
    }

    public void UpdateItem(int id, Item updatedItem)
    {
        if (Items.ContainsKey(id))
        {
            Items[id] = updatedItem;
        }
    }

    public void ClearInventory()
    {
        Items.Clear();
    }

    public void Dispose()
    {
        if (Items.IsCreated)
        {
            Items.Dispose();
        }
    }
}

[Serializable]
public class ItemPrefab
{
    public int Id = -1; // Уникальный ID
    public AssetReference Link; // Ссылка на ресурс

    public void SetId(int id)
    {
        Id = id;
    }
}
