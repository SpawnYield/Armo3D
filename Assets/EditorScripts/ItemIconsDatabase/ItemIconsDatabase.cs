using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;

[CreateAssetMenu(fileName = "ItemIconsDatabase", menuName = "Database/ItemIconsDatabase")]
public class ItemIconsDatabase : ScriptableObject
{
    // Глобальный статический список всех предметов
    [HideInInspector]
    public List<ItemIcon> ItemIcons = new List<ItemIcon>();
    public static List<ItemIcon> GlobalItemiconList = new List<ItemIcon>();
    // Очередь свободных ID
    private static HashSet<int> freeIds = new HashSet<int>();

    // Получение уникального ID
    public static int GetNextId()
    {
        int id = 0;
        while (true)
        {
            id++;
            if (freeIds.Contains(id))
            {
                continue;
            }
            freeIds.Add(id);
            break;
        }
        return id;
    }

    // Добавление предмета в локальный список
    public void AddItem(ItemIcon item)
    {
        item.SetId(GetNextId());
        // Добавляем в локальный список
        ItemIcons.Add(item);
        Debug.Log($"Item с ID {item.Id} добавлен в локальный список.");
    }

    public void RemoveItem(int id)
    {
        // Ищем элемент с нужным ID
        var item = ItemIcons.Find(x => x.Id == id);
        if (item != null)
        {
            // Удаляем найденный элемент
            ItemIcons.Remove(item);
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
            var itemsToRemove = new List<ItemIcon>(ItemIcons);

            foreach (var item in itemsToRemove)
            {
                ItemIcons.Remove(item);
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
        foreach (var item in ItemIcons)
        {
            // Проверяем, чтобы не добавить элемент в глобальный список, если его уже там нет (по ID)
            if (!GlobalItemiconList.Exists(existingItem => existingItem.Id == item.Id))
            {
                GlobalItemiconList.Add(item);
            }
        }

        Debug.Log("Все локальные предметы добавлены в глобальный список.");
    }
    // Метод для удаления предмета из глобального списка по ID
    public void RemoveFromGlobalList(int itemId)
    {
        // Находим элемент в глобальном списке по ID
        var itemToRemove = GlobalItemiconList.FirstOrDefault(existingItem => existingItem.Id == itemId);

        if (itemToRemove != null)
        {
            // Удаляем элемент, если он найден
            GlobalItemiconList.Remove(itemToRemove);
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
        GlobalItemiconList.Clear();
        Debug.Log("Глобальный список очищен.");
    }

    public static void PrintGlobalList()
    {
        Debug.Log($"GlobalItemList Size:{GetGlobalItemList().Count};");
    }

    public static List<ItemIcon> GetGlobalItemList()
    {
        return GlobalItemiconList;
    }
}
[Serializable]
public class ItemIcon
{
    public int Id = -1; // Уникальный ID
    public AssetReference Link; // Ссылка на ресурс

    public void SetId(int id)
    {
        Id = id;
    }
}
