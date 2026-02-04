using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Cámara FPS")]
    public float mouseSensitivity = 20f;
    private float xRotation = 0f;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Vector2 lookInput;

    [Header("Disparar")]
    public GameObject bulletPrefab;
    public Transform firePosition;
    public float bulletSpeed = 20f;

    private InputSystemActions input;
    private Camera mainCam;

    private void Awake()
    {
        input = new InputSystemActions();
    }

    void Start()
    {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.GetComponent<CameraBehaviour>().SetTarget(this.transform);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        RegisterInputEvents();

        if (IsOwner)
            EnableInput();

        if (IsServer)
            MoveToRandomSpawnPoint();
    }

    private void RegisterInputEvents()
    {
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        input.Player.Fire.performed += OnFire;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        if (mainCam != null)
        {
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            mainCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        transform.position += move * moveSpeed * Time.deltaTime;
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
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

    private void MoveToRandomSpawnPoint()
    {
        Transform spawnPoint = RespawnManager.Instance.GetRandomPoint();
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner) DisableInput();
    }

    private void EnableInput() => input.Player.Enable();
    private void DisableInput() => input.Player.Disable();
}