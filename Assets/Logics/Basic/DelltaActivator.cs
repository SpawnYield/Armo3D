using System.Collections;
using UnityEngine;

public class DelltaActivator
{
    public static void EnableForTime(GameObject Target_Prefab, float Time) => EnableForTimeCouroutine( new(Time), Target_Prefab);
    private static IEnumerator EnableForTimeCouroutine(WaitForSeconds time,GameObject prefab)
    {
        prefab.SetActive(true);
        yield return time;
        prefab.SetActive(false);
        yield return null;
    }
}
