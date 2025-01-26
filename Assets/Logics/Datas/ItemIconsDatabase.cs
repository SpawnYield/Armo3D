using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ItemIconsDatabase", menuName = "Database/ItemIconsDatabase")]

public class ItemIconsDatabase : ScriptableObject
{
    [HideInInspector]
    public List<ItemPrefab> ItemIcons = new List<ItemPrefab>(); // Локальный список предметов
    private static HashSet<int> freeIds = new HashSet<int>(); // Список свободных ID

    public void Awake()
    {
        Debug.Log(ItemIcons);
    }
    // Получение уникального ID
    public static int GetNextId()
    {
        int id = 1;
        while (freeIds.Contains(id))
        {
            id++;
        }
        freeIds.Add(id);
        return id;
    }

    // Добавление предмета в локальный список
    public void AddItem(ItemPrefab item)
    {
        item.SetId(GetNextId());
        ItemIcons.Add(item);
        Debug.Log($"Item с ID {item.Id} добавлен в локальный список.");
    }

    // Удаление предмета из локального списка
    public void RemoveItem(int id)
    {
        var item = ItemIcons.Find(x => x.Id == id);
        if (item != null)
        {
            ItemIcons.Remove(item);
            freeIds.Remove(id);
            Debug.Log($"Item с ID {id} удален из локального списка.");
        }
        else
        {
            Debug.LogWarning($"Item с ID {id} не найден в локальном списке.");
        }
    }

    // Очистка локального списка
    public void ClearLocalList()
    {
        foreach (var item in ItemIcons)
        {
            freeIds.Remove(item.Id);
        }
        ItemIcons.Clear();
        Debug.Log("Локальный список очищен.");
    }

    // Объединение локального списка с глобальным
    public void MergeToGlobalList()
    {
        foreach (var item in ItemIcons)
        {
            if (!CentralData.centralData.globalItemIconList.Any(existingItem => existingItem.Id == item.Id))
            {
                CentralData.centralData.globalItemIconList.Add(item);
            }
        }

        Debug.Log("Все локальные предметы добавлены в глобальный список.");
    }

    // Удаление предмета из глобального списка
    public void RemoveFromGlobalList(int itemId)
    {

        var itemToRemove = CentralData.centralData.globalItemIconList.FirstOrDefault(x => x.Id == itemId);
        if (itemToRemove != null)
        {
            CentralData.centralData.globalItemIconList.Remove(itemToRemove);
            Debug.Log($"Предмет с ID {itemId} удален из глобального списка.");
        }
        else
        {
            Debug.LogWarning($"Предмет с ID {itemId} не найден в глобальном списке.");
        }
    }

    // Очистка глобального списка
    public static void ClearGlobalList()
    {
        CentralData.centralData.globalItemIconList.Clear();
        Debug.Log("Глобальный список очищен.");
    }
    // Получение ссылки на ресурс по ID
    public static AssetReference GetImageAssetToID(int id)
    {
        var item = CentralData.centralData.globalItemIconList.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            Debug.LogWarning($"Item с ID {id} не найден в глобальном списке.");
            return null;
        }

        return item.Link;
    }

    // Печать глобального списка
    public static void PrintGlobalList()
    {
        Debug.Log($"GlobalItemList Size: {CentralData.centralData.globalItemIconList.Count}");
    }

    // Получение глобального списка
    public static List<ItemPrefab> GetGlobalItemList()
    {
        return CentralData.centralData.globalItemIconList;
    }

}
