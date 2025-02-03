using System.Collections.Generic;
using UnityEngine;

public class AoeDamager : MonoBehaviour
{
    private readonly HashSet<Humanoid> damagedHumanoids = new ();
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageRadius = 1f;
    [SerializeField] private Humanoid owner;
    // Свойства для управления переменными (если нужно)
    public float Damage
    {
        get { return damage; }
        set { damage = Mathf.Max(0, value); } // Ограничение на положительное значение
    }

    public float DamageRadius
    {
        get { return damageRadius; }
        set { damageRadius = Mathf.Max(0, value); } // Ограничение на положительное значение
    }

    public Humanoid Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    public LayerMask TargetLayers
    {
        get { return targetLayers; }
        set { targetLayers = value; }
    }

    public void ApplyDamage()
    {
        damagedHumanoids.Clear();
        RaycastHit[] hitResults = new RaycastHit[300]; // Максимум 10 объектов (можно изменить)
        Vector3 direction = Vector3.forward; // Направление луча (можно задать любое)

        int hitCount = Physics.SphereCastNonAlloc(
            transform.position,     // Начальная позиция сферы
            damageRadius,           // Радиус сферы
            direction,              // Направление движения
            hitResults,             // Массив для результатов
            damageRadius,           // Длина сферы (0 = только начальная точка)
            targetLayers            // Слой, по которому ищем
        );

        for (int i = 0; i < hitCount; i++)
        {
            hitResults[i].collider.TryGetComponent(out Humanoid humanoid);
            // Если Humanoid не найден или уже был поврежден — пропускаем
            if (humanoid == null || damagedHumanoids.Contains(humanoid))
                continue;
            Debug.Log($"{damage} Damaged!");
            // Добавляем в список поврежденных и наносим урон
            damagedHumanoids.Add(humanoid);
            humanoid.TakeDamage(damage, owner);
            Debug.Log("Попадание: " + hitResults[i].collider.name);
        }


    }
    // Вызывается при активации объекта
    public void OnEnable()
    {
       
        ApplyDamage();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(Vector3.zero), DamageRadius);
    }
}
