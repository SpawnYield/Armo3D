using System.Collections.Generic;
using UnityEngine;

public class AoeDamager : MonoBehaviour
{
    private readonly HashSet<Humanoid> damagedHumanoids = new ();
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageRadius = 1f;
    [SerializeField] private Humanoid owner;
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
        RaycastHit[] hitResults = new RaycastHit[300]; // �������� 10 �������� (����� ��������)
        Vector3 direction = Vector3.forward; // ����������� ���� (����� ������ �����)

        int hitCount = Physics.SphereCastNonAlloc(
            transform.position,     // ��������� ������� �����
            damageRadius,           // ������ �����
            direction,              // ����������� ��������
            hitResults,             // ������ ��� �����������
            damageRadius,           // ����� ����� (0 = ������ ��������� �����)
            targetLayers            // ����, �� �������� ����
        );

        for (int i = 0; i < hitCount; i++)
        {
            hitResults[i].collider.TryGetComponent(out Humanoid humanoid);
            // ���� Humanoid �� ������ ��� ��� ��� ��������� � ����������
            if (humanoid == null || damagedHumanoids.Contains(humanoid))
                continue;
            Debug.Log($"{damage} Damaged!");
            // ��������� � ������ ������������ � ������� ����
            damagedHumanoids.Add(humanoid);
            humanoid.TakeDamage(damage, owner);
            Debug.Log("���������: " + hitResults[i].collider.name);
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
