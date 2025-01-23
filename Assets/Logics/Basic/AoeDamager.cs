using System.Collections.Generic;
using UnityEngine;

public class AoeDamager : MonoBehaviour
{
    private List<Humanoid> DamagedHumanoids = new();
    public float Damage = 1f;
    public float DamageRadius = 1f;
    public Humanoid Owner;
    [SerializeField]
    private LayerMask TargetLayers;

    public void OnEnable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DamageRadius, TargetLayers);
        foreach (var hitCollider in hitColliders)
        {
           
            hitCollider.gameObject.TryGetComponent(out Humanoid humanoid);
            if (humanoid == null) continue;
            if (DamagedHumanoids.Contains(humanoid)) continue;

            DamagedHumanoids.Add(humanoid);
            humanoid.TakeDamage(Damage, Owner);
        }
    }
    public void OnDisable()
    {
        DamagedHumanoids.Clear();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(Vector3.zero), DamageRadius);
    }

}
