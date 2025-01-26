using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CentralData", menuName = "Scriptable Objects/CentralData")]
public class CentralData : ScriptableObject
{
    public List<ItemPrefab> GlobalItemList = new List<ItemPrefab>();
    public List<ItemPrefab> globalItemIconList = new List<ItemPrefab>(); // Глобальный список предметов
    public static CentralData centralData;
    private void OnEnable()
    {
        centralData = this;
    }
    private void OnValidate()
    {
        centralData = this;
    }
}
