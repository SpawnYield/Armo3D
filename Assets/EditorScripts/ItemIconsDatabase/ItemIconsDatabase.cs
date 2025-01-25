using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;

[CreateAssetMenu(fileName = "ItemIconsDatabase", menuName = "Database/ItemIconsDatabase")]
public class ItemIconsDatabase : ScriptableObject
{
    // ���������� ����������� ������ ���� ���������
    [HideInInspector]
    public List<ItemIcon> ItemIcons = new List<ItemIcon>();
    public static List<ItemIcon> GlobalItemiconList = new List<ItemIcon>();
    // ������� ��������� ID
    private static HashSet<int> freeIds = new HashSet<int>();

    // ��������� ����������� ID
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

    // ���������� �������� � ��������� ������
    public void AddItem(ItemIcon item)
    {
        item.SetId(GetNextId());
        // ��������� � ��������� ������
        ItemIcons.Add(item);
        Debug.Log($"Item � ID {item.Id} �������� � ��������� ������.");
    }

    public void RemoveItem(int id)
    {
        // ���� ������� � ������ ID
        var item = ItemIcons.Find(x => x.Id == id);
        if (item != null)
        {
            // ������� ��������� �������
            ItemIcons.Remove(item);
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
            var itemsToRemove = new List<ItemIcon>(ItemIcons);

            foreach (var item in itemsToRemove)
            {
                ItemIcons.Remove(item);
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
        foreach (var item in ItemIcons)
        {
            // ���������, ����� �� �������� ������� � ���������� ������, ���� ��� ��� ��� ��� (�� ID)
            if (!GlobalItemiconList.Exists(existingItem => existingItem.Id == item.Id))
            {
                GlobalItemiconList.Add(item);
            }
        }

        Debug.Log("��� ��������� �������� ��������� � ���������� ������.");
    }
    // ����� ��� �������� �������� �� ����������� ������ �� ID
    public void RemoveFromGlobalList(int itemId)
    {
        // ������� ������� � ���������� ������ �� ID
        var itemToRemove = GlobalItemiconList.FirstOrDefault(existingItem => existingItem.Id == itemId);

        if (itemToRemove != null)
        {
            // ������� �������, ���� �� ������
            GlobalItemiconList.Remove(itemToRemove);
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
        GlobalItemiconList.Clear();
        Debug.Log("���������� ������ ������.");
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
    public int Id = -1; // ���������� ID
    public AssetReference Link; // ������ �� ������

    public void SetId(int id)
    {
        Id = id;
    }
}
