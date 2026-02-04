using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider))]
public class Bullet : NetworkBehaviour
{
    public float lifetime = 3f;
    private float lifeTimer = 0f;
    public int damage = 10;

    void Update()
    {
        if (!IsServer) return;
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime)
        {
            NetworkObject.Despawn();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;

        HealthBar vida = other.GetComponentInParent<HealthBar>();
        if (vida != null)
        {
            vida.ApplyDamage(damage);
        }
        NetworkObject.Despawn();
    }
}
