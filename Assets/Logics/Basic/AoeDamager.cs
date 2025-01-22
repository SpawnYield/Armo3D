using System.Collections.Generic;
using UnityEngine;

public class AoeDamager : MonoBehaviour
{
    private List<Humanoid> DamagedHumanoids = new();
    public float Damage = 1f;
    public Humanoid Owner;

    private void OnDisable()
    {
        DamagedHumanoids.Clear();
    }
    private void OnCollisionStay(Collision collision)
    {
        collision.transform.TryGetComponent<Humanoid>(out Humanoid humanoid);
        if (humanoid = null) return;
        if (DamagedHumanoids.Contains(humanoid)) return;
        DamagedHumanoids.Add(humanoid);
        humanoid.TakeDamage(Damage, Owner);
    }
}
