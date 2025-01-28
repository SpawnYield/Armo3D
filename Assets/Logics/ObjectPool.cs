using System.Collections.Generic;
using UnityEngine;

public class UniversalObjectPool
{
    private readonly Dictionary<object, Queue<GameObject>> pools = new Dictionary<object, Queue<GameObject>>();
    private readonly Transform parent; // Родительский объект для иерархии
    private readonly int defaultInitialCount = 5; // Размер пула по умолчанию

    public UniversalObjectPool(Transform parent = null, int defaultInitialCount = 5)
    {
        this.parent = parent;
        this.defaultInitialCount = defaultInitialCount;
    }

    /// <summary>
    /// Получение объекта из пула.
    /// Если пула для данного объекта нет — он будет автоматически зарегистрирован.
    /// </summary>
    public T GetFromPool<T>(T prefab) where T : Object
    {
        var key = (object)prefab;

        // Автоматическая регистрация, если пул ещё не создан
        if (!pools.ContainsKey(key))
        {
            RegisterPool(prefab, defaultInitialCount);
        }

        var pool = pools[key];
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            // Если пул пустой, создаём новый объект
            obj = CreateObject(prefab);
        }

        obj.SetActive(true);
        return (T)(prefab is MonoBehaviour ? obj.GetComponent<T>() : obj as Object);
    }

    /// <summary>
    /// Возвращение объекта в пул.
    /// </summary>
    public void ReturnToPool<T>(T prefab, GameObject obj) where T : Object
    {
        var key = (object)prefab;

        if (!pools.ContainsKey(key))
        {
            Debug.LogError($"Пул для {prefab.name} не зарегистрирован.");
            return;
        }

        obj.SetActive(false);
        pools[key].Enqueue(obj);
    }

    /// <summary>
    /// Явная регистрация пула для объекта (необязательно, используется при автозапросе).
    /// </summary>
    public void RegisterPool<T>(T prefab, int initialCount) where T : Object
    {
        var key = (object)prefab;

        if (pools.ContainsKey(key))
        {
            Debug.LogWarning($"Пул для {prefab.name} уже зарегистрирован.");
            return;
        }

        var queue = new Queue<GameObject>();
        pools[key] = queue;

        for (int i = 0; i < initialCount; i++)
        {
            var obj = CreateObject(prefab);
            ReturnToPool(prefab, obj);
        }
    }

    /// <summary>
    /// Создаёт новый объект.
    /// </summary>
    private GameObject CreateObject<T>(T prefab) where T : Object
    {
        GameObject obj;

        if (prefab is GameObject gameObjectPrefab)
        {
            obj = Object.Instantiate(gameObjectPrefab, parent);
        }
        else if (prefab is MonoBehaviour monoBehaviourPrefab)
        {
            obj = Object.Instantiate(monoBehaviourPrefab.gameObject, parent);
        }
        else
        {
            throw new System.Exception($"Тип {typeof(T)} не поддерживается пулом.");
        }

        obj.SetActive(false);
        return obj;
    }
}
