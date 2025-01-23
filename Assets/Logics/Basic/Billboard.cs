using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // Получаем основную камеру
    }

    private void Update()
    {
        // Вращаем объект так, чтобы он всегда смотрел на камеру
        Vector3 targetPosition = new Vector3(mainCamera.transform.position.x, transform.position.y, mainCamera.transform.position.z);
        transform.LookAt(targetPosition);
    }
}
