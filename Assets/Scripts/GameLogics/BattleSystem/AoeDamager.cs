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
    // �������� ��� ���������� ����������� (���� �����)
    public float Damage
    {
        get { return damage; }
        set { damage = Mathf.Max(0, value); } // ����������� �� ������������� ��������
    }

    public float DamageRadius
    {
        get { return damageRadius; }
        set { damageRadius = Mathf.Max(0, value); } // ����������� �� ������������� ��������
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
        // ��������� ��� ���������� � �������
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius, targetLayers);
        foreach (var hitCollider in hitColliders)
        {
            // �������� ��������� Humanoid, ���� �� ����
            hitCollider.gameObject.TryGetComponent(out Humanoid humanoid);

            // ���� Humanoid �� ������ ��� ��� ��� ��������� � ����������
            if (humanoid == null || damagedHumanoids.Contains(humanoid))
                continue;

            // ��������� � ������ ������������ � ������� ����
            damagedHumanoids.Add(humanoid);
            humanoid.TakeDamage(damage, owner);
        }
    }
    // ���������� ��� ��������� �������
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
