using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public int enemiesToSpawn = 3;
    public bool allowRepeatPoints = false;

    public override void OnNetworkSpawn()
    {
        // IMPORTANT: només el servidor spawneja
        if (!IsServer) return;

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        // Si no permetem repetir punts, limitem el màxim
        int count = enemiesToSpawn;
        if (!allowRepeatPoints)
            count = Mathf.Min(enemiesToSpawn, spawnPoints.Length);

        // Fem una còpia per “barrejar” punts sense repetir
        int[] indices = new int[spawnPoints.Length];
        for (int i = 0; i < indices.Length; i++) indices[i] = i;

        // Shuffle simple (Fisher-Yates)
        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = indices[i];
            indices[i] = indices[j];
            indices[j] = tmp;
        }

        for (int k = 0; k < count; k++)
        {
            Transform p = allowRepeatPoints
                ? spawnPoints[Random.Range(0, spawnPoints.Length)]
                : spawnPoints[indices[k]];

            GameObject enemy = Instantiate(enemyPrefab, p.position, p.rotation);
            enemy.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}


