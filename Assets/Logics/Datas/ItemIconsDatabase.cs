
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemIconsDatabase", menuName = "Database/ItemIconsDatabase")]

public class ItemIconsDatabase : ScriptableObject
{
    [HideInInspector]
    public List<ItemPrefab> ItemIcons = new List<ItemPrefab>(); // Локальный список предметов
    public CentralData centralDataLink;
    private static HashSet<int> freeIds = new HashSet<int>(); // Список свободных ID
    private bool isInitialized = false; // Флаг инициализации

    public void OnEnable() => Init();
    public void OnValidate() => Init();
    public void Awake() => Init();
    private void Init()
    {
        // Инициализация только если не было выполнено ранее
        if (isInitialized) return;
        Debug.Log("inited ItemIconsDatabase");
        MergeToGlobalList();
        isInitialized = true; // Устанавливаем флаг инициализации
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
        try
        {
            foreach (var item in ItemIcons)
            {
                // Проверяем, чтобы не добавить элемент в глобальный список, если его уже там нет (по ID)
                if (!centralDataLink.globalItemIconList.Exists(existingItem => existingItem.Id == item.Id))
                {
                    centralDataLink.globalItemIconList.Add(item);
                }
            }
            Debug.Log("Все локальные предметы добавлены в глобальный список.");
        }
        catch (Exception ex)
        {
            Debug.Log($"Ошибка:{ex}");
        }

    }

    // Удаление предмета из глобального списка
    public void RemoveFromGlobalList(int itemId)
    {

        var itemToRemove = centralDataLink.globalItemIconList.FirstOrDefault(x => x.Id == itemId);
        if (itemToRemove != null)
        {
            centralDataLink.globalItemIconList.Remove(itemToRemove);
            Debug.Log($"Предмет с ID {itemId} удален из глобального списка.");
        }
        else
        {
            Debug.LogWarning($"Предмет с ID {itemId} не найден в глобальном списке.");
        }
    }

    // Очистка глобального списка
    public void ClearGlobalList()
    {
        centralDataLink.globalItemIconList.Clear();
        Debug.Log("Глобальный список очищен.");
    }

    // Печать глобального списка
    public void PrintGlobalList()
    {
        Debug.Log($"GlobalItemList Size: {centralDataLink.globalItemIconList.Count}");
    }

    // Получение глобального списка
    public List<ItemPrefab> GetGlobalItemList()
    {
        return centralDataLink.globalItemIconList;
    }

}
