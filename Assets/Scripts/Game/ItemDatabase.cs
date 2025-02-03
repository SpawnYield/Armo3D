using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/Item Database")]
[Serializable]
public class ItemDatabase : ScriptableObject
{
    // ���������� ����������� ������ ���� ���������
    [HideInInspector] 
    public List<ItemPrefab> Items = new List<ItemPrefab>();
    // ������� ��������� ID
    private static HashSet<int> freeIds = new HashSet<int>();
    public void Init()
    {
        Debug.Log("inited ItemDatabase");
        MergeToGlobalList();
    }


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
        try
        {
            foreach (var item in Items)
            {
                // ���������, ����� �� �������� ������� � ���������� ������, ���� ��� ��� ��� ��� (�� ID)
                if (!CentralData.GlobalItemList.Exists(existingItem => existingItem.Id == item.Id))
                {
                    CentralData.GlobalItemList.Add(item);
                }
            }
            Debug.Log("��� ��������� �������� ��������� � ���������� ������.");
        }
        catch (Exception ex)
        {
            Debug.Log($"������:{ex}");
        }



    }
    // ����� ��� �������� �������� �� ����������� ������ �� ID
    public void RemoveFromGlobalList(int itemId)
    {
        // ������� ������� � ���������� ������ �� ID
        var itemToRemove = CentralData.GlobalItemList.FirstOrDefault(existingItem => existingItem.Id == itemId);

        if (itemToRemove != null)
        {
            // ������� �������, ���� �� ������
            CentralData.GlobalItemList.Remove(itemToRemove);
            Debug.Log("������� � ID " + itemId + " ������ �� ����������� ������.");
        }
        else
        {
            // ���� ������� �� ������
            Debug.LogWarning("������� � ID " + itemId + " �� ������ � ���������� ������.");
        }
    }

    // ������� ����������� ������
    public void ClearGlobalList()
    {
        CentralData.GlobalItemList.Clear();
        Debug.Log("���������� ������ ������.");
    }

    public void PrintGlobalList()
    {
        Debug.Log($"GlobalItemList Size:{GetGlobalItemList().Count};");
    }

    public List<ItemPrefab> GetGlobalItemList()
    {
        return CentralData.GlobalItemList;
    }
}

