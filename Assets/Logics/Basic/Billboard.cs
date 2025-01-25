
using System.Threading.Tasks;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    private Transform thisTransform;
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main; // �������� �������� ������
        TargetsUpdateAsync(); // ��������� ����������� ����������
    }

    private bool OnUpdate = false;

    private async void TargetsUpdateAsync()
    {
        while (this != null && gameObject != null) // ���������, ��� ������ �� ���������
        {
            try
            {
                // ���������� ��������� ����
                thisTransform = EntityManager.GetTarget(25f, transform.position, SortType.None);
                OnUpdate = thisTransform != null;

                // ��������� ��������� Canvas
                if (canvas != null)
                {
                    canvas.enabled = OnUpdate;
                }
            }
            catch (MissingReferenceException)
            {
                break; // ��������� ����, ���� ������ ���������
            }

            await Task.Delay(1200); // ��������� ��� � �������
        }
    }

    private void Update()
    {
        if (!OnUpdate || mainCamera == null)
            return;

        // ������������� ������� ��� Billboard � ������ ����������
        Vector3 directionToCamera = transform.position - mainCamera.transform.position;

        // ������������ ��������� ������, ����������� ��� Y
        directionToCamera.y = 0; // ����� ������������ ����������
        if (directionToCamera.sqrMagnitude < 0.01f) // ��������� ������� ����� ����������
        {
            directionToCamera = mainCamera.transform.forward; // ������������� ����������� ������
        }

        // ������������ ������ �� ������
        transform.rotation = Quaternion.LookRotation(-directionToCamera.normalized);
    }

}
