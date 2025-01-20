
using UnityEngine;
using Random = Unity.Mathematics.Random;


public class WorldManager : MonoBehaviour
{

    public int worldWidthChunks = 16;
    public int ChunksWidth = 32;
    public int worldHeightUpper = 32;
    public int worldHeightLower = -512;

    public int seed = 12345;

    private static Random random;



    public void GenerateWorld()
    {
        ClearWorld();

        random = new Random((uint)seed);

        for (int Xm = -worldWidthChunks; Xm <= worldWidthChunks; Xm++)
        {
            // Создаем GameObject для Tilemap
            GameObject mapObject = new GameObject("Tilemap");
            Custom_Map map = mapObject.AddComponent<Custom_Map>();
            mapObject.isStatic = true;
            int Xposition = Xm * (ChunksWidth*2);
            mapObject.transform.SetParent(transform);
            mapObject.transform.position = new Vector2(Xposition, 0f);

            for (int x = -ChunksWidth ; x <= ChunksWidth ; x++)
            {
                for (int y = worldHeightLower; y <= worldHeightUpper; y++)
                {
                    map.AddBlock(0, new(x,y) , gameObject.layer);
                }           
            }

        }
    }
    public void ClearWorld()
    {
        try
        {
            foreach (Custom_Map child in transform.GetComponentsInChildren<Custom_Map>())
            {
                if (Application.isEditor && !Application.isPlaying)
                {
                    DestroyImmediate(child.gameObject);
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }
        }
        catch { }
    }


    public void SaveWorld()
    {

    }
    public void LoadWorld()
    {

    }
    [System.Serializable]
    public class ChunkData
    {
        public string playerName;
        public int level;
        public float health;
    }

}
