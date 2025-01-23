using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // �������� �������� ������
    }

    private void Update()
    {
        // ������� ������ ���, ����� �� ������ ������� �� ������
        Vector3 targetPosition = new Vector3(mainCamera.transform.position.x, transform.position.y, mainCamera.transform.position.z);
        transform.LookAt(targetPosition);
    }
}
