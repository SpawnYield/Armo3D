using System.Collections;
using UnityEngine;

public class Bullets : MonoBehaviour
{


    public GameObject Bullet;
    [Range(1, 1000)]
    public int Count = 10000;
    [Range(1, 1000)]
    public int Row = 10000;
    [Range(100, 10000000)]
    public int Iterations = 10000;
    [Range(0.0002f, 1f)]
    public float Interval = 1.0f;
    [Range(0.01f, 3f)]
    public float LifeTime = 0.5f;
    [Range(0.01f, 30f)]
    public float Power = 0.5f;
    public bool Pooling = false;

    
    private UniversalObjectPool objectPool;

    private void Start()
    {
        objectPool = new UniversalObjectPool(transform); // Создаём универсальный пул
        for (int i = 0; i < Count; i++)
        {
            for (int j = 0; j < Row; j++)
            { 
                if (Pooling)
                {
                    StartCoroutine(Looppooling(new(Interval), Iterations, i,j));

                }
                else
                {
                    StartCoroutine(Loop(new(Interval), Iterations, i,j));

                }
            }
        }
    }
    private IEnumerator Loop(WaitForSeconds interval,int iterations,int offset,int offsetRow)
    {
        Vector3 spawnPosition = new (offset, offsetRow, 0f);
        Vector3 LinearVector = new(0, 0f, Power);
        while (true)
        {
            GameObject bullet = Instantiate(Bullet, spawnPosition, Quaternion.identity); // Создаем пулю
            Rigidbody rb = bullet.GetComponent<Rigidbody>(); 
            rb.linearVelocity = LinearVector; // Применяем скорость

            StartCoroutine(BulletLifetime(bullet));
            yield return interval;
        }

    }
    private IEnumerator BulletLifetime(GameObject bullet)
    {
        yield return new WaitForSeconds(LifeTime);
        Destroy(bullet);
    }

    private IEnumerator Looppooling(WaitForSeconds interval, int iterations, int offset,int offsetRow)
    {
        Vector3 spawnPosition = new(offset, offsetRow, 0f);
        Vector3 LinearVector = new(0, 0f, Power);
        while (true)
        {
            GameObject bullet = objectPool.GetFromPool(Bullet);
            bullet.transform.position = spawnPosition;
            bullet.transform.rotation = Quaternion.identity;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.linearVelocity = LinearVector; // Применяем скорость
            StartCoroutine(BulletPoolLifetime(bullet));
            yield return interval;
        }
    }
    private IEnumerator BulletPoolLifetime(GameObject bullet)
    {
        yield return new WaitForSeconds(LifeTime);
        objectPool.ReturnToPool(Bullet, bullet);
    }

}
