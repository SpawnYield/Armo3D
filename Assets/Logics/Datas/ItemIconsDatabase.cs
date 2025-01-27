
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemIconsDatabase", menuName = "Database/ItemIconsDatabase")]
[Serializable]
public class ItemIconsDatabase : ScriptableObject
{
    [HideInInspector]
    public List<ItemPrefab> ItemIcons = new List<ItemPrefab>(); // ��������� ������ ���������
    private static HashSet<int> freeIds = new HashSet<int>(); // ������ ��������� ID

    public void Init()
    {
        Debug.Log("inited ItemIconsDatabase");
        MergeToGlobalList();

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
        try
        {
            foreach (var item in ItemIcons)
            {
                // ���������, ����� �� �������� ������� � ���������� ������, ���� ��� ��� ��� ��� (�� ID)
                if (!CentralData.globalItemIconList.Exists(existingItem => existingItem.Id == item.Id))
                {
                    CentralData.globalItemIconList.Add(item);
                }
            }
            Debug.Log("��� ��������� �������� ��������� � ���������� ������.");
        }
        catch (Exception ex)
        {
            Debug.Log($"������:{ex}");
        }

    }

    // �������� �������� �� ����������� ������
    public void RemoveFromGlobalList(int itemId)
    {

        var itemToRemove = CentralData.globalItemIconList.FirstOrDefault(x => x.Id == itemId);
        if (itemToRemove != null)
        {
            CentralData.globalItemIconList.Remove(itemToRemove);
            Debug.Log($"������� � ID {itemId} ������ �� ����������� ������.");
        }
        else
        {
            Debug.LogWarning($"������� � ID {itemId} �� ������ � ���������� ������.");
        }
    }

    // ������� ����������� ������
    public void ClearGlobalList()
    {
        CentralData.globalItemIconList.Clear();
        Debug.Log("���������� ������ ������.");
    }

    // ������ ����������� ������
    public void PrintGlobalList()
    {
        Debug.Log($"GlobalItemList Size: {CentralData.globalItemIconList.Count}");
    }

    // ��������� ����������� ������
    public List<ItemPrefab> GetGlobalItemList()
    {
        return CentralData.globalItemIconList;
    }

}
