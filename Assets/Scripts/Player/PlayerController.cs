using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float rotateSpeed = 3.0f;
    [SerializeField] private float dropHoldTime = 0.5f;
    [Header("Links")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private FieldOfView fovCircle;

    private float horizontal, vertical;
    private float diagLimiter = 0.7f;
    private Rigidbody2D rb;

    private ItemSlotManager itemSlotManager;
    private PlayerInputAction _input;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        itemSlotManager = GetComponent<ItemSlotManager>();

        if (!IsOwner)
        {
            gameObject.layer = LayerMask.NameToLayer("BehindMask");
            return;
        }

        EnableInput();
    }

    private void EnableInput()
    {
        _input = new PlayerInputAction();
        _input.Player.Enable();

        _input.Player.Primary.performed += HandleAttack;
        _input.Player.Secondary.performed += HandleAttack;

        _input.Player.DropPrimary.started += HandleDropThrowHold;
        _input.Player.DropSecondary.started += HandleDropThrowHold;
        _input.Player.DropPrimary.performed += PerformDropThrow;
        _input.Player.DropSecondary.performed += PerformDropThrow;
    }

    public override void OnNetworkSpawn()
    {
        PlayerSpawnManager.Instance.OnGameStarted?.Invoke();
        PlayerSpawnManager.Instance.networkPlayersSpawned.Add(GetComponentInParent<NetworkObject>());

    }

    public override void OnNetworkDespawn()
    {
        PlayerSpawnManager.Instance.networkPlayersSpawned[(int)OwnerClientId] = null;
    }
    void Update()
    {
        if (!IsOwner) return;
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        fieldOfView.SetOrigin(new Vector3(transform.position.x, transform.position.y, 0));
        fovCircle.SetOrigin(new Vector3(transform.position.x, transform.position.y, 0));
    }

    private void HandleRotation()
    {
        //Mouse rotation
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var targetAngle = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        Quaternion currentAngle = transform.rotation;
        transform.rotation = Quaternion.Lerp(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);

        // Handle adjusting FoV's
        float finalAngle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        var finalDir = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), dir.z);
        fieldOfView.SetAimDirection(finalDir);
        fovCircle.SetAimDirection(finalDir);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        //Move player
        if (horizontal != 0 && vertical != 0)
        {
            horizontal *= diagLimiter;
            vertical *= diagLimiter;
        }
        rb.velocity = new Vector2(horizontal * playerSpeed, vertical * playerSpeed);
    }

    private void HandleAttack(InputAction.CallbackContext obj)
    {
        if (obj.duration >= dropHoldTime) return;
        //Button is released before 0.5 seconds

        if (obj.action == _input.Player.Primary)
        {
            if (itemSlotManager.weaponInstance != null) itemSlotManager.weaponInstance.GetComponent<ItemClass>().Use();
        }
        if (obj.action == _input.Player.Secondary)
        {
            if (itemSlotManager.pickupInstance != null) itemSlotManager.pickupInstance.GetComponent<ItemClass>().Use();
        }
    }

    private void HandleDropThrowHold(InputAction.CallbackContext obj)
    {
        if (obj.duration < dropHoldTime) return;
        // Button is held for more than 0.5 seconds
        
    }

    private void PerformDropThrow(InputAction.CallbackContext obj)
    {
        if (obj.duration < dropHoldTime) return;

        if (obj.action == _input.Player.DropPrimary)
        {
            if (itemSlotManager.weaponInstance != null) itemSlotManager.weaponInstance.GetComponent<ItemClass>().DropItemServerRpc("weapon");
        }
        if (obj.action == _input.Player.DropSecondary)
        {
            if (itemSlotManager.pickupInstance != null) itemSlotManager.pickupInstance.GetComponent<ItemClass>().DropItemServerRpc("pickup");
        }
    }


}
