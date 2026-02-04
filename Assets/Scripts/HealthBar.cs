using Unity.Netcode;
using UnityEngine;

public class HealthBar : NetworkBehaviour
{

    public enum DeathBehaviour { Respawn, Despawn }
    public DeathBehaviour deathBehaviour = DeathBehaviour.Respawn;


    public int maxHealth = 100;
    public float respawnDelay = 2f;

    public NetworkVariable<int> currentHealth =
        new NetworkVariable<int>(100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    private bool dead;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            dead = false;
        }
    }

    public void ApplyDamage(int damage)
    {
        if (!IsServer) return;
        if (dead) return;

        int newHealth = Mathf.Max(0, currentHealth.Value - damage);
        currentHealth.Value = newHealth;

        if (newHealth == 0)
        {
            dead = true;
            //Invoke(nameof(RespawnServer), respawnDelay);

            if (deathBehaviour == DeathBehaviour.Respawn)
            {
                Invoke(nameof(RespawnServer), respawnDelay);
            }
            else // Despawn
            {
                Invoke(nameof(DespawnServer), 0.2f);
            }

        }
    }

    private void DespawnServer()
    {
        if (!IsServer) return;

        // Només objectes de xarxa es poden despawnejar
        NetworkObject no = GetComponent<NetworkObject>();
        if (no != null && no.IsSpawned)
            no.Despawn(true);
    }



    private void RespawnServer()
    {
        if (!IsServer) return;

        // Reset vida al servidor (això ho veuen tots)
        currentHealth.Value = maxHealth;
        dead = false;

        // Triar punt de respawn
        Transform p = RespawnManager.Instance != null ? RespawnManager.Instance.GetRandomPoint() : null;
        Vector3 pos = p != null ? p.position : Vector3.zero;
        Quaternion rot = p != null ? p.rotation : Quaternion.identity;

        // Ordenar a l'owner que es teletransporti
        RespawnRpc(pos, rot);
    }

    // RPC modern: enviar només a l'Owner d'aquest NetworkObject
    [Rpc(SendTo.Owner)]
    private void RespawnRpc(Vector3 position, Quaternion rotation)
    {
        // Teleport local (owner). Els altres ho veuran via NetworkTransform owner-authoritative.
        transform.SetPositionAndRotation(position, rotation);
    }
}