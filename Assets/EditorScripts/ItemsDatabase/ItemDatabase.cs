using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/Item Database")]

public class ItemDatabase : ScriptableObject
{
    // ���������� ����������� ������ ���� ���������
    [HideInInspector] 
    public List<ItemPrefab> Items = new List<ItemPrefab>();
    public static List<ItemPrefab> GlobalItemList = new List<ItemPrefab>();
    // ������� ��������� ID
    private static HashSet<int> freeIds = new HashSet<int>();

    // ��������� ����������� ID
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

    // ���������� �������� � ��������� ������
    public void AddItem(ItemPrefab item)
    {
        item.SetId(GetNextId());
        // ��������� � ��������� ������
        Items.Add(item);
        Debug.Log($"Item � ID {item.Id} �������� � ��������� ������.");
    }

    public void RemoveItem(int id)
    {
        // ���� ������� � ������ ID
        var item = Items.Find(x => x.Id == id);
        if (item != null)
        {
            // ������� ��������� �������
            Items.Remove(item);
            freeIds.Remove(item.Id);
        }
    }

    // ������� ���������� ������ ��� ������ ���������� ID
    public void ClearLocalList()
    {
        string result = "��������� ������ ������.";
        try
        {
            // ������� ����� ������, ����� ��������� ������� ��������
            var itemsToRemove = new List<ItemPrefab>(Items);

            foreach (var item in itemsToRemove)
            {
                Items.Remove(item);
                freeIds.Remove(item.Id);
            }

            Debug.Log("��������� ������ ������, ID �����������.");
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

    // ����������� ���� ��������� �� ���������� ������ � ����������
    public void MergeToGlobalList()
    {
        foreach (var item in Items)
        {
            // ���������, ����� �� �������� ������� � ���������� ������, ���� ��� ��� ��� ��� (�� ID)
            if (!GlobalItemList.Exists(existingItem => existingItem.Id == item.Id))
            {
                GlobalItemList.Add(item);
            }
        }

        Debug.Log("��� ��������� �������� ��������� � ���������� ������.");
    }
    // ����� ��� �������� �������� �� ����������� ������ �� ID
    public void RemoveFromGlobalList(int itemId)
    {
        // ������� ������� � ���������� ������ �� ID
        var itemToRemove = GlobalItemList.FirstOrDefault(existingItem => existingItem.Id == itemId);

        if (itemToRemove != null)
        {
            // ������� �������, ���� �� ������
            GlobalItemList.Remove(itemToRemove);
            Debug.Log("������� � ID " + itemId + " ������ �� ����������� ������.");
        }
        else
        {
            // ���� ������� �� ������
            Debug.LogWarning("������� � ID " + itemId + " �� ������ � ���������� ������.");
        }
    }

    // ������� ����������� ������
    public static void ClearGlobalList()
    {
        GlobalItemList.Clear();
        Debug.Log("���������� ������ ������.");
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
    public int Id = -1; // ���������� ID
    public AssetReference Link; // ������ �� ������

    public void SetId(int id)
    {
        Id = id;
    }
}
