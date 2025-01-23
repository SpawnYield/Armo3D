using System.Collections;
using UnityEngine;

public class DelltaActivator
{
    public static void EnableForTime(GameObject targetObject, float time)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object is null!");
            return;
        }

        // Активируем объект
        targetObject.SetActive(true);

        // Запускаем корутину для деактивации объекта через некоторое время
        targetObject.GetComponent<MonoBehaviour>().StartCoroutine(EnableForTimeCoroutine(new WaitForSeconds(time), targetObject));
    }

    private static IEnumerator EnableForTimeCoroutine(WaitForSeconds waitTime, GameObject targetObject)
    {
        // Ожидаем указанное время
        yield return waitTime;

        // Деактивируем объект
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }
}
