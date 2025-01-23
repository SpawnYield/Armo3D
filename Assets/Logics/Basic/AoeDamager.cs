using System.Collections.Generic;
using UnityEngine;

public class AoeDamager : MonoBehaviour
{
    private List<Humanoid> DamagedHumanoids = new();
    public float Damage = 1f;
    public Humanoid Owner;
    [SerializeField]
    private LayerMask TargetLayers;
    [SerializeField]
    private SphereCollider _collider;
    public void OnEnable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_collider.center + transform.position, _collider.radius, TargetLayers);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.gameObject.TryGetComponent(out Humanoid humanoid);
            if (humanoid == null) return;
            Debug.Log($"Humanoid:{humanoid}");
            if (DamagedHumanoids.Contains(humanoid)) return;
            DamagedHumanoids.Add(humanoid);
            humanoid.TakeDamage(Damage, Owner);
        }
    }
    public void OnDisable()
    {
        Debug.Log($"DamagedHumanoids:Cleared!");
        DamagedHumanoids.Clear();
    }
    private void OnCollisionStay(Collision collision) => TakeDamae(collision);
    private void OnCollisionEnter(Collision collision) => TakeDamae(collision);
    private void OnCollisionExit(Collision collision)=> TakeDamae(collision);

    private void TakeDamae(Collision collision)
    {
        Debug.Log($"TakeDamae:{1}");
        collision.gameObject.TryGetComponent(out Humanoid humanoid);
    
        Debug.Log($"Humanoid:{humanoid}");
        if (humanoid == null) return;
        if (DamagedHumanoids.Contains(humanoid)) return;
        DamagedHumanoids.Add(humanoid);
        humanoid.TakeDamage(Damage, Owner);
    }
}
