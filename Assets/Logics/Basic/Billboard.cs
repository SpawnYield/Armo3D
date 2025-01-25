
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
        mainCamera = Camera.main; // Получаем основную камеру
        TargetsUpdateAsync(); // Запускаем асинхронное обновление
    }

    private bool OnUpdate = false;

    private async void TargetsUpdateAsync()
    {
        while (this != null && gameObject != null) // Проверяем, что объект не уничтожен
        {
            try
            {
                // Асинхронно обновляем цель
                thisTransform = EntityManager.GetTarget(25f, transform.position, SortType.None);
                OnUpdate = thisTransform != null;

                // Обновляем состояние Canvas
                if (canvas != null)
                {
                    canvas.enabled = OnUpdate;
                }
            }
            catch (MissingReferenceException)
            {
                break; // Прерываем цикл, если объект уничтожен
            }

            await Task.Delay(1200); // Обновляем раз в секунду
        }
    }

    private void Update()
    {
        if (!OnUpdate || mainCamera == null)
            return;

        // Устанавливаем позицию для Billboard с учётом ориентации
        Vector3 directionToCamera = transform.position - mainCamera.transform.position;

        // Обеспечиваем видимость сверху, корректируя ось Y
        directionToCamera.y = 0; // Сброс вертикальной компоненты
        if (directionToCamera.sqrMagnitude < 0.01f) // Проверяем слишком малые расстояния
        {
            directionToCamera = mainCamera.transform.forward; // Устанавливаем направление камеры
        }

        // Поворачиваем объект на камеру
        transform.rotation = Quaternion.LookRotation(-directionToCamera.normalized);
    }

}
