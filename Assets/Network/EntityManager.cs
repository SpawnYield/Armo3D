using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum SortType
{
    None,
    Low_Hp,
    High_Hp,
    Range
}

public class EntityManager : MonoBehaviour
{
    private readonly static List<Transform> Targets = new();

    private SynchronizationContext unityContext;
    private void Start()
    {
        unityContext = SynchronizationContext.Current;
        TargetsUpdateAsync(200);
    }

    private async void TargetsUpdateAsync(int intervalMs)
    {
        while (true)
        {
            unityContext.Post(_ =>
            {
                if(Targets!=null)
                    Targets.Clear();
            }, null);
            foreach (EnemyTargetComponent result in FindObjectsByType<EnemyTargetComponent>(FindObjectsSortMode.None))
            {
                if (result != null && result.transform != null && Targets != null && unityContext != null)
                {
                    if (result.Died) continue;
                    unityContext.Post(_ =>
                    {
                        Targets.Add(result.transform);
                    }, null);
                }
            }
            await Task.Delay(intervalMs);
        }
    }
    public static Transform GetTarget(float _Distance, Vector3 _Position, SortType sortType)
    {
        foreach (Transform result in Targets)
        {
            switch (sortType)
            {
                case SortType.Low_Hp:
                    {

                    }
                    break;
                case SortType.High_Hp:
                    {

                    }
                    break;
                default:
                    {
                        if (Vector3.Distance(_Position, result!=null? result.position:Vector3.zero) <= _Distance)
                        {
                            return result;
                        }
                    }
                    break;
            }
        }
        return null;
    }

}
