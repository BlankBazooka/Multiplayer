using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    public Transform[] respawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetRandomPoint()
    {
        if (respawnPoints == null || respawnPoints.Length == 0) return null;
        int idx = Random.Range(0, respawnPoints.Length);
        return respawnPoints[idx];
    }
}

