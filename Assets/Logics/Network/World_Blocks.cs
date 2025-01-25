
using System.Collections.Generic;
using UnityEngine;

public class World_Blocks : MonoBehaviour
{
    public static World_Blocks Instance;
    public void Init()
    {
        Instance = this;    
    }
    [Header("Prefabs of Blocks")]
    public List<GameObject> Blocks;
}
