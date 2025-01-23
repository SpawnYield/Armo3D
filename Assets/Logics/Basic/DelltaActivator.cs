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

        // ���������� ������
        targetObject.SetActive(true);

        // ��������� �������� ��� ����������� ������� ����� ��������� �����
        targetObject.GetComponent<MonoBehaviour>().StartCoroutine(EnableForTimeCoroutine(new WaitForSeconds(time), targetObject));
    }

    private static IEnumerator EnableForTimeCoroutine(WaitForSeconds waitTime, GameObject targetObject)
    {
        // ������� ��������� �����
        yield return waitTime;

        // ������������ ������
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }
}
