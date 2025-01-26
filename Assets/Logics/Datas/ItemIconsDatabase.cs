using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ItemIconsDatabase", menuName = "Database/ItemIconsDatabase")]

public class ItemIconsDatabase : ScriptableObject
{
    [HideInInspector]
    public List<ItemPrefab> ItemIcons = new List<ItemPrefab>(); // ��������� ������ ���������
    private static HashSet<int> freeIds = new HashSet<int>(); // ������ ��������� ID

    public void Awake()
    {
        Debug.Log(ItemIcons);
    }
    // ��������� ����������� ID
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

    // ���������� �������� � ��������� ������
    public void AddItem(ItemPrefab item)
    {
        item.SetId(GetNextId());
        ItemIcons.Add(item);
        Debug.Log($"Item � ID {item.Id} �������� � ��������� ������.");
    }

    // �������� �������� �� ���������� ������
    public void RemoveItem(int id)
    {
        var item = ItemIcons.Find(x => x.Id == id);
        if (item != null)
        {
            ItemIcons.Remove(item);
            freeIds.Remove(id);
            Debug.Log($"Item � ID {id} ������ �� ���������� ������.");
        }
        else
        {
            Debug.LogWarning($"Item � ID {id} �� ������ � ��������� ������.");
        }
    }

    // ������� ���������� ������
    public void ClearLocalList()
    {
        foreach (var item in ItemIcons)
        {
            freeIds.Remove(item.Id);
        }
        ItemIcons.Clear();
        Debug.Log("��������� ������ ������.");
    }

    // ����������� ���������� ������ � ����������
    public void MergeToGlobalList()
    {
        foreach (var item in ItemIcons)
        {
            if (!CentralData.centralData.globalItemIconList.Any(existingItem => existingItem.Id == item.Id))
            {
                CentralData.centralData.globalItemIconList.Add(item);
            }
        }

        Debug.Log("��� ��������� �������� ��������� � ���������� ������.");
    }

    // �������� �������� �� ����������� ������
    public void RemoveFromGlobalList(int itemId)
    {

        var itemToRemove = CentralData.centralData.globalItemIconList.FirstOrDefault(x => x.Id == itemId);
        if (itemToRemove != null)
        {
            CentralData.centralData.globalItemIconList.Remove(itemToRemove);
            Debug.Log($"������� � ID {itemId} ������ �� ����������� ������.");
        }
        else
        {
            Debug.LogWarning($"������� � ID {itemId} �� ������ � ���������� ������.");
        }
    }

    // ������� ����������� ������
    public static void ClearGlobalList()
    {
        CentralData.centralData.globalItemIconList.Clear();
        Debug.Log("���������� ������ ������.");
    }
    // ��������� ������ �� ������ �� ID
    public static AssetReference GetImageAssetToID(int id)
    {
        var item = CentralData.centralData.globalItemIconList.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            Debug.LogWarning($"Item � ID {id} �� ������ � ���������� ������.");
            return null;
        }

        return item.Link;
    }

    // ������ ����������� ������
    public static void PrintGlobalList()
    {
        Debug.Log($"GlobalItemList Size: {CentralData.centralData.globalItemIconList.Count}");
    }

    // ��������� ����������� ������
    public static List<ItemPrefab> GetGlobalItemList()
    {
        return CentralData.centralData.globalItemIconList;
    }

}
