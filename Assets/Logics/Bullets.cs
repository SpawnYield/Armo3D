using System.Collections;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

public class Bullets : MonoBehaviour
{
    public GameObject Bullet;
    [Range(1, 1000)] public int Count = 100;
    [Range(1, 1000)] public int Row = 100;
    [Range(0.0002f, 1f)] public float Interval = 0.1f;
    [Range(0.01f, 3f)] public float LifeTime = 0.5f;
    [Range(0.01f, 30f)] public float Power = 0.5f;
    public bool Pooling = false;
    public bool JobSystem = true;

    private UniversalObjectPool objectPool;
    private NativeArray<Vector3> bulletPositions;
    private NativeArray<Vector3> bulletVelocities;
    private NativeArray<float> bulletTimers;
    private GameObject[] bullets;
    private bool isRunning = false;

    private void Start()
    {
        if (Pooling) objectPool = new UniversalObjectPool(transform);

        if (JobSystem) InitJobSystem();
        else InitCoroutine();
    }

    private void InitCoroutine()
    {
        // Спаун пуль будет осуществляться как раньше
        for (int i = 0; i < Count; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                if (Pooling) StartCoroutine(LoopPooling(new WaitForSeconds(Interval), i, j));
                else StartCoroutine(Loop(new WaitForSeconds(Interval), i, j));
            }
        }
    }

    private void InitJobSystem()
    {
        int totalBullets = Count * Row;
        bulletPositions = new NativeArray<Vector3>(totalBullets, Allocator.Persistent);
        bulletVelocities = new NativeArray<Vector3>(totalBullets, Allocator.Persistent);
        bulletTimers = new NativeArray<float>(totalBullets, Allocator.Persistent);
        bullets = new GameObject[totalBullets];

        // Спаун пуль через пул
        for (int i = 0; i < Count; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                Vector3 spawnPosition = new Vector3(i, j, 0);
                bulletPositions[i] = spawnPosition;
                bulletVelocities[i] = new Vector3(0, 0, Power);
                bulletTimers[i] = -1f;
                bullets[i] = Instantiate(Bullet, spawnPosition, Quaternion.identity);
                bullets[i].SetActive(false);
            }
        }
        isRunning = true;
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        WaitForSeconds interval = new(Interval);
        while (true)
        {
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    if (bulletTimers[i] <= 0) // Если пуля неактивна
                    {
                        bulletPositions[i] = new Vector3(i, j, 0f); // Обновляем позицию
                        bulletTimers[i] = LifeTime; // Сбрасываем таймер жизни
                        if (bullets[i] != null)
                        {
                            bullets[i].transform.position = bulletPositions[i]; // Обновляем позицию пули
                            bullets[i].SetActive(true); // Активируем пулю
                        }
                    }
                }
            }
            yield return interval;
        }
    }

    private void Update()
    {
        if (JobSystem && isRunning)
        {
            BulletMoveJob moveJob = new()
            {
                positions = bulletPositions,
                velocities = bulletVelocities,
                timers = bulletTimers,
                deltaTime = Time.deltaTime,
                lifeTime = LifeTime
            };

            JobHandle handle = moveJob.Schedule(bulletPositions.Length, 32);
            handle.Complete();

            // Обновляем положение пуль в основном потоке после завершения работы Job
            for (int i = 0; i < bulletPositions.Length; i++)
            {
                if (bulletTimers[i] > 0)
                {
                    if (bullets[i] != null)
                        bullets[i].transform.position = bulletPositions[i];
                }
                else
                {
                    if (bullets[i] != null)
                    {
                        bullets[i].SetActive(false); // Деактивируем пулю
                        bulletTimers[i] = -1f; // Сбрасываем таймер
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (JobSystem)
        {
            bulletPositions.Dispose();
            bulletVelocities.Dispose();
            bulletTimers.Dispose();
        }
    }

    private IEnumerator Loop(WaitForSeconds interval, int offset, int offsetRow)
    {
        Vector3 spawnPosition = new(offset, offsetRow, 0f);
        Vector3 velocity = new(0, 0, Power);

        while (true)
        {
            GameObject bullet = Instantiate(Bullet, spawnPosition, Quaternion.identity);
            StartCoroutine(BulletLifetime(bullet, velocity));
            yield return interval;
        }
    }

    private IEnumerator LoopPooling(WaitForSeconds interval, int offset, int offsetRow)
    {
        Vector3 spawnPosition = new(offset, offsetRow, 0f);
        Vector3 velocity = new(0, 0, Power);

        while (true)
        {
            GameObject bullet = objectPool.GetFromPool(Bullet);
            bullet.transform.position = spawnPosition;
            bullet.transform.rotation = Quaternion.identity;
            StartCoroutine(BulletPoolLifetime(bullet, velocity));
            yield return interval;
        }
    }

    private IEnumerator BulletLifetime(GameObject bullet, Vector3 velocity)
    {
        float timer = 0f;
        while (timer < LifeTime)
        {
            bullet.transform.position += velocity * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(bullet);
    }

    private IEnumerator BulletPoolLifetime(GameObject bullet, Vector3 velocity)
    {
        float timer = 0f;
        while (timer < LifeTime)
        {
            bullet.transform.position += velocity * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        objectPool.ReturnToPool(Bullet, bullet);
    }

    [BurstCompile]
    private struct BulletMoveJob : IJobParallelFor
    {
        public NativeArray<Vector3> positions;
        [ReadOnly] public NativeArray<Vector3> velocities;
        public NativeArray<float> timers;
        public float deltaTime;
        public float lifeTime;

        public void Execute(int index)
        {
            if (timers[index] > 0)
            {
                positions[index] += velocities[index] * deltaTime;
                timers[index] -= deltaTime;
            }
        }
    }
}
