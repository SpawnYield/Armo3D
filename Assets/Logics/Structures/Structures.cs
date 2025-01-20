using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

public static class Structures { }


public struct Item 
{
    public int ItemImageId;
    public int ItemId;
    public int ItemCount;
    public int ItemPrice;
    public Item(int _ItemImageId, int _ItemId, int _ItemCount,int _ItemPrice)
    {
        ItemImageId = _ItemImageId;
        ItemId = _ItemId;
        ItemCount = _ItemCount;
        ItemPrice = _ItemPrice;
    }
    public static Item operator +(Item a, Item b)
    {
        if (a.ItemId == b.ItemId)
        {
            return new Item(a.ItemImageId, a.ItemId, a.ItemCount + b.ItemCount,a.ItemPrice);
        }
        else
        {
            return a;
        }
    }
}


public class Inventory
{
    public List<Item> Items;
    public void AddItem(Item item)
    {
        Item result = ContainsItem(item.ItemId);
        if(result.ItemImageId < 0 && result.ItemId < 0 && result.ItemCount < 0 && result.ItemPrice <0)
        {
            Items.Add(item);
            return;
        }else
        {
            Items.Remove(result);

            Items.Add(item+ result);
        }
        
    }
    public Item ContainsItem(int id)
    {
        // Пример запуска джоба для поиска элемента
        using (NativeArray<Item> itemsNativeArray = new NativeArray<Item>(Items.ToArray(), Allocator.TempJob))
        {
            FindItem job = new FindItem
            {
                Items = itemsNativeArray,
                SearchId = id,
                Found = new NativeArray<Item>(1, Allocator.TempJob)
            };

            JobHandle handle = job.Schedule();
            handle.Complete();

            Item result = job.Found[0];
            job.Found.Dispose();
            return result;
        }
    }


}

[BurstCompile(CompileSynchronously = true)]
public struct FindItem : IJob
{
    [ReadOnly] public NativeArray<Item> Items;
    public int SearchId; // ID для поиска
    public NativeArray<Item> Found; // Результат поиска

    public void Execute()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].ItemId == SearchId)
            {
                Found[0] = Items[i];
                return;
            }
        }
        Found[0] = new(-1,-1,-1,-1);
    }
 
}

