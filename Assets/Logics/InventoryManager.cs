

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    [Header("CentralData")]
    public CentralData _CentralData;

    [Header("Inventory Limit:24,576 Различных Объектов")]
    [SerializeField]
    private GameObject _Item;
    
    private static Inventory _inventory;
    private SynchronizationContext unityContext;

    public void Start()
    {
        unityContext = SynchronizationContext.Current;
        _inventory = new();
        // Получаем все компоненты Image в дочерних объектах
        _inventory.OnItemAdded += ItemAdded;
        _inventory.OnItemRemoved += ItemDeleted;
        Image[] images = GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            Destroy(image.transform); // Удаляем компонент
        }
        for (int i = 0; i < 2457; i++)
        {
            Item newItem = new(i, 1, 1, 1, 1, 1, 1);

            _inventory.AddItem(newItem);
        }


        InventoryLoad();
    }

    private void ItemAdded(Item _item)
    {

    }
    private void ItemDeleted(int Id)
    {

    }
    private CancellationTokenSource _cancellationTokenSource; // Источник токенов отмены

    private async void InventoryLoad()
    {
        _cancellationTokenSource = new CancellationTokenSource(); // Создаем источник токенов

        try
        {
            await Task.Delay(1000);
            Item[] items = _inventory.GetAllItems().ToArray();
            foreach (var item in items)
            {
                // Проверка на отмену перед каждой асинхронной операцией
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                AssetReference reference = _CentralData.GetAssetToID(item.ImageId,1);
                var handle = Addressables.LoadAssetAsync<Sprite>(reference);
                await handle.Task; // Ожидаем завершения загрузки

                // Проверяем, если задача была отменена, выбрасываем исключение
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Debug.Log("Загрузка инвентаря была отменена.");
                    break;
                }

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    unityContext.Post(_ =>
                    {
                        if (_Item)
                        {
                            GameObject newUiprefab = Instantiate(_Item);
                            try
                            {
                                newUiprefab.GetComponent<Image>().sprite = Instantiate(handle.Result);
                                newUiprefab.transform.SetParent(transform, false);
                            }
                            catch
                            {
                                try
                                {
#if UNITY_EDITOR
                                    DestroyImmediate(newUiprefab);
#else
                                    Destroy(newUiprefab);
#endif
                                }
                                catch
                                {
                                    Debug.Log($"Ошибка Удаления");
                                }
                            }
                        }
                    }, null);
                }
                else
                {
                    Debug.Log($"Ошибка загрузки текстуры: {handle.OperationException}");
                }

                Debug.Log(item);
                await Task.Delay(4);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Загрузка инвентаря была отменена.");
        }
        finally
        {
            // Очистка ресурсов при завершении
            _cancellationTokenSource.Dispose();
        }
    }
    public void OnDestroy()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }
    }

}
