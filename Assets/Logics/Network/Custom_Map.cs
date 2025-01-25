using Unity.Mathematics;
using UnityEngine;

public class Custom_Map : MonoBehaviour
{
    public void AddBlock(int Id,int2 Position,int layerId)
    {
        if(Id < 0)
            return;
        GameObject newBlock = Instantiate(World_Blocks.Instance.Blocks[Id]);
        newBlock.transform.SetParent(transform, false);
        newBlock.transform.position =new Vector2(Position.x,Position.y);
        newBlock.layer = layerId;
        newBlock.isStatic = true;
    }
}
