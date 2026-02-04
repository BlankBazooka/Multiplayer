using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : NetworkBehaviour
{
    private InputSystemActions input;
    private Vector2 move;

    [Header("Movement")]
    public float turnSpeed = 150f;
    public float moveSpeed = 3f;

    [Header("Disparar")]
    public GameObject bulletPrefab;
    public Transform firePosition;
    public float bulletSpeed = 10f;

    void Start()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.GetComponent<CameraBehaviour>().SetTarget(this.transform);
        }
    }
    private void Awake()
    {
        input = new InputSystemActions();
    }

    public override void OnNetworkSpawn()
    {
        // Sempre podem registrar events aquí;
        // però només habilitarem input si som Owner.
        RegisterInputEvents();

        if (IsOwner)
            EnableInput();
    }

    public override void OnGainedOwnership()
    {
        // si l'ownership arriba després, activem input aquí.
        EnableInput();
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
            DisableInput();
    }

    private void RegisterInputEvents()
    {
        // Evita dobles subscripcions si OnNetworkSpawn es crida en re-spawn
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMoveCanceled;

        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMoveCanceled;

        input.Player.Fire.performed -= OnFire;
        input.Player.Fire.performed += OnFire;
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;
        ShootServerRpc();
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePosition.forward * bulletSpeed;
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    private void EnableInput()
    {
        input.Player.Enable();
    }

    private void DisableInput()
    {
        input.Player.Disable();
        move = Vector2.zero;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        move = Vector2.zero;
    }

    private void Update()
    {
        if (!IsOwner) return;

        // A/D: girar (move.x)
        transform.Rotate(0f, move.x * turnSpeed * Time.deltaTime, 0f);

        // W/S: avançar (move.y)
        transform.Translate(0f, 0f, move.y * moveSpeed * Time.deltaTime);

    }
}


