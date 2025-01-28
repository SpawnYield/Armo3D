using System.Collections.Generic;
using UnityEngine;

public class UniversalObjectPool
{
    private readonly Dictionary<object, Queue<GameObject>> pools = new Dictionary<object, Queue<GameObject>>();
    private readonly Transform parent; // ������������ ������ ��� ��������
    private readonly int defaultInitialCount = 5; // ������ ���� �� ���������

    public UniversalObjectPool(Transform parent = null, int defaultInitialCount = 5)
    {
        this.parent = parent;
        this.defaultInitialCount = defaultInitialCount;
    }

    /// <summary>
    /// ��������� ������� �� ����.
    /// ���� ���� ��� ������� ������� ��� � �� ����� ������������� ���������������.
    /// </summary>
    public T GetFromPool<T>(T prefab) where T : Object
    {
        var key = (object)prefab;

        // �������������� �����������, ���� ��� ��� �� ������
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
            // ���� ��� ������, ������ ����� ������
            obj = CreateObject(prefab);
        }

        obj.SetActive(true);
        return (T)(prefab is MonoBehaviour ? obj.GetComponent<T>() : obj as Object);
    }

    /// <summary>
    /// ����������� ������� � ���.
    /// </summary>
    public void ReturnToPool<T>(T prefab, GameObject obj) where T : Object
    {
        var key = (object)prefab;

        if (!pools.ContainsKey(key))
        {
            Debug.LogError($"��� ��� {prefab.name} �� ���������������.");
            return;
        }

        obj.SetActive(false);
        pools[key].Enqueue(obj);
    }

    /// <summary>
    /// ����� ����������� ���� ��� ������� (�������������, ������������ ��� �����������).
    /// </summary>
    public void RegisterPool<T>(T prefab, int initialCount) where T : Object
    {
        var key = (object)prefab;

        if (pools.ContainsKey(key))
        {
            Debug.LogWarning($"��� ��� {prefab.name} ��� ���������������.");
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
    /// ������ ����� ������.
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
            throw new System.Exception($"��� {typeof(T)} �� �������������� �����.");
        }

        obj.SetActive(false);
        return obj;
    }
}
