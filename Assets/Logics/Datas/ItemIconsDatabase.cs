
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemIconsDatabase", menuName = "Database/ItemIconsDatabase")]

public class ItemIconsDatabase : ScriptableObject
{
    [HideInInspector]
    public List<ItemPrefab> ItemIcons = new List<ItemPrefab>(); // ��������� ������ ���������
    public CentralData centralDataLink;
    private static HashSet<int> freeIds = new HashSet<int>(); // ������ ��������� ID
    private bool isInitialized = false; // ���� �������������

    public void OnEnable() => Init();
    public void OnValidate() => Init();
    public void Awake() => Init();
    private void Init()
    {
        // ������������� ������ ���� �� ���� ��������� �����
        if (isInitialized) return;
        Debug.Log("inited ItemIconsDatabase");
        MergeToGlobalList();
        isInitialized = true; // ������������� ���� �������������
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
                if (!centralDataLink.globalItemIconList.Exists(existingItem => existingItem.Id == item.Id))
                {
                    centralDataLink.globalItemIconList.Add(item);
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

        var itemToRemove = centralDataLink.globalItemIconList.FirstOrDefault(x => x.Id == itemId);
        if (itemToRemove != null)
        {
            centralDataLink.globalItemIconList.Remove(itemToRemove);
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
        centralDataLink.globalItemIconList.Clear();
        Debug.Log("���������� ������ ������.");
    }

    // ������ ����������� ������
    public void PrintGlobalList()
    {
        Debug.Log($"GlobalItemList Size: {centralDataLink.globalItemIconList.Count}");
    }

    // ��������� ����������� ������
    public List<ItemPrefab> GetGlobalItemList()
    {
        return centralDataLink.globalItemIconList;
    }

}
