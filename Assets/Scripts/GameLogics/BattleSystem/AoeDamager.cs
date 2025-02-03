using System.Collections.Generic;
using UnityEngine;

public class AoeDamager : MonoBehaviour
{
    [SerializeField]
    private LayerMask targetLayers;
    private HashSet<Humanoid> damagedHumanoids = new HashSet<Humanoid>();
    private float damage = 1f;
    private float damageRadius = 1f;
    private Humanoid owner;
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
        // Считываем все коллайдеры в радиусе
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius, targetLayers);
        foreach (var hitCollider in hitColliders)
        {
            // Получаем компонент Humanoid, если он есть
            hitCollider.gameObject.TryGetComponent(out Humanoid humanoid);

            // Если Humanoid не найден или уже был поврежден — пропускаем
            if (humanoid == null || damagedHumanoids.Contains(humanoid))
                continue;

            // Добавляем в список поврежденных и наносим урон
            damagedHumanoids.Add(humanoid);
            humanoid.TakeDamage(damage, owner);
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
