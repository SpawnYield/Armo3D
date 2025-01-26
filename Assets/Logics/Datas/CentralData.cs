using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "CentralData", menuName = "Database/CentralData")]
[Serializable]
public class CentralData : ScriptableObject
{
    public List<ItemPrefab> GlobalItemList = new List<ItemPrefab>();
    public List<ItemPrefab> globalItemIconList = new List<ItemPrefab>(); // Глобальный список предметов

    public AssetReference GetAssetToID(int id,int Type)
    {
        AssetReference result = null;
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
                break ;
            }
        }


        return result;
    }

}
