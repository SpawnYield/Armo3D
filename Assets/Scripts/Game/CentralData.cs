using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "CentralData", menuName = "Database/CentralData")]

public class CentralData : ScriptableObject
{
    [HideInInspector] static public List<ItemPrefab> GlobalItemList = new List<ItemPrefab>();
    [HideInInspector] static public List<ItemPrefab> globalItemIconList = new List<ItemPrefab>(); // Глобальный список предметов
    private static bool Inited = false;
    public List<ItemDatabase> ItemDataBaseList = new List<ItemDatabase>();
    public List<ItemIconsDatabase> ItemImageDataBaseList = new List<ItemIconsDatabase>(); // Глобальный список предметов

    public void InitAll()
    {
        ItemDataBaseList.ForEach(item => { item.Init(); });
        ItemImageDataBaseList.ForEach(item => { item.Init(); });
        Inited = true;
    }
    
    public AssetReference GetAssetToID(int id,int Type)
    {
        if(!Inited)
        {
            InitAll();
        }
        AssetReference result = null;
        try
        {

            switch (Type)
            {
                case 1:
                    {
                        var item = globalItemIconList.FirstOrDefault(x => x.Id == id);
                        if (item == null)
                        {
                            Debug.LogWarning($"Item с ID {id} не найден в глобальном списке.");
                            return null;
                        }
                        result = item.Link;
                        break;
                    }
                default:
                    {
                        var item = GlobalItemList.FirstOrDefault(x => x.Id == id);
                        if (item == null)
                        {
                            Debug.LogWarning($"Item с ID {id} не найден в глобальном списке.");
                            return null;
                        }
                        result = item.Link;
                        break;
                    }
            }
        }
        catch (Exception e) {
            Debug.Log(e);
        }
        return result;
    }

}
