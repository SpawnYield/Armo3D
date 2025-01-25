using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/Item Database")]

public class ItemDatabase : ScriptableObject
{
    // Глобальный статический список всех предметов
    [HideInInspector] 
    public List<ItemPrefab> Items = new List<ItemPrefab>();
    public static List<ItemPrefab> GlobalItemList = new List<ItemPrefab>();
    // Очередь свободных ID
    private static HashSet<int> freeIds = new HashSet<int>();

    // Получение уникального ID
    public static int GetNextId()
    {
        int id = 0;
        while (true)
        {
            id++;
            if(freeIds.Contains(id))
            {
                continue;
            }
            freeIds.Add(id);
            break;
        }
        return id;
    }

    // Добавление предмета в локальный список
    public void AddItem(ItemPrefab item)
    {
        item.SetId(GetNextId());
        // Добавляем в локальный список
        Items.Add(item);
        Debug.Log($"Item с ID {item.Id} добавлен в локальный список.");
    }

    public void RemoveItem(int id)
    {
        // Ищем элемент с нужным ID
        var item = Items.Find(x => x.Id == id);
        if (item != null)
        {
            // Удаляем найденный элемент
            Items.Remove(item);
            freeIds.Remove(item.Id);
        }
    }

    // Очистка локального списка без сброса глобальных ID
    public void ClearLocalList()
    {
        string result = "Локальный список очищен.";
        try
        {
            // Создаем копию списка, чтобы безопасно удалить предметы
            var itemsToRemove = new List<ItemPrefab>(Items);

            foreach (var item in itemsToRemove)
            {
                Items.Remove(item);
                freeIds.Remove(item.Id);
            }

            Debug.Log("Локальный список очищен, ID освобождены.");
        }
        catch (Exception ex)
        {
            result = ex.ToString();
        }
        finally
        {
            Debug.Log(result);
        }
    }

    // Объединение всех предметов из локального списка в глобальный
    public void MergeToGlobalList()
    {
        foreach (var item in Items)
        {
            // Проверяем, чтобы не добавить элемент в глобальный список, если его уже там нет (по ID)
            if (!GlobalItemList.Exists(existingItem => existingItem.Id == item.Id))
            {
                GlobalItemList.Add(item);
            }
        }

        Debug.Log("Все локальные предметы добавлены в глобальный список.");
    }
    // Метод для удаления предмета из глобального списка по ID
    public void RemoveFromGlobalList(int itemId)
    {
        // Находим элемент в глобальном списке по ID
        var itemToRemove = GlobalItemList.FirstOrDefault(existingItem => existingItem.Id == itemId);

        if (itemToRemove != null)
        {
            // Удаляем элемент, если он найден
            GlobalItemList.Remove(itemToRemove);
            Debug.Log("Предмет с ID " + itemId + " удален из глобального списка.");
        }
        else
        {
            // Если элемент не найден
            Debug.LogWarning("Предмет с ID " + itemId + " не найден в глобальном списке.");
        }
    }

    // Очистка глобального списка
    public static void ClearGlobalList()
    {
        GlobalItemList.Clear();
        Debug.Log("Глобальный список очищен.");
    }

    public static void PrintGlobalList()
    {
        Debug.Log($"GlobalItemList Size:{GetGlobalItemList().Count};");
    }

    public static List<ItemPrefab> GetGlobalItemList()
    {
        return GlobalItemList;
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
