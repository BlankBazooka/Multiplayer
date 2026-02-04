using UnityEngine;
using Unity.Netcode;

public class EnemyIA : NetworkBehaviour
{
    [Header("Configuración")]
    public float moveSpeed = 2.5f;
    public float detectionRange = 10f;
    public float wanderRange = 5f;
    public LayerMask playerLayer;

    public int dañoPorGolpe = 10;
    public float velocidadAtaque = 1.0f;
    private float timerAtaque;

    [Header("Deambular")]
    private Vector3 targetWanderPos;
    private float wanderTimer;
    public float wanderInterval = 4f;

    private Transform targetPlayer;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { enabled = false; return; }
        SetNewWanderTarget();
    }

    void Update()
    {
        if (!IsServer) return;

        FindNearestPlayer();

        if (targetPlayer != null)
        {
            MoverHacia(targetPlayer.position);
        }
        else
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval || Vector3.Distance(transform.position, targetWanderPos) < 0.5f)
            {
                SetNewWanderTarget();
                wanderTimer = 0;
            }
            MoverHacia(targetWanderPos);
        }
    }

    void MoverHacia(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void SetNewWanderTarget()
    {
        Vector2 randomPoint = Random.insideUnitCircle * wanderRange;
        targetWanderPos = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
    }

    void FindNearestPlayer()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        if (players.Length > 0)
        {
            targetPlayer = players[0].transform;
        }
        else
        {
            targetPlayer = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsServer) return;

        if (Time.time >= timerAtaque)
        {
            HealthBar vidaJugador = other.GetComponentInParent<HealthBar>();

            if (vidaJugador != null)
            {
                vidaJugador.ApplyDamage(dañoPorGolpe);
                timerAtaque = Time.time + velocidadAtaque;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}