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
    private ThrowItemInputHandler _throwItemInputHandler;

    public GameEvent_SO playerSpawned;
    public GameEvent_SO playerDied;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        itemSlotManager = GetComponent<ItemSlotManager>();
        _throwItemInputHandler = transform.parent.GetComponentInChildren<ThrowItemInputHandler>();

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

        _input.Player.Primary.performed += HandlePrimarySecondaryCheck;
        _input.Player.Secondary.performed += HandlePrimarySecondaryCheck;

        _throwItemInputHandler.dropHoldTime = dropHoldTime;
        _input.Player.Primary.performed += _throwItemInputHandler.HandleThrowInput;
        _input.Player.Secondary.performed += _throwItemInputHandler.HandleThrowInput;
        _input.Player.ThrowPrimary.performed += _throwItemInputHandler.HandleStartThrowInput;
        _input.Player.ThrowSecondary.performed += _throwItemInputHandler.HandleStartThrowInput;
    }

    public override void OnNetworkSpawn()
    {
        //playerSpawned.Raise(this, GetComponentInParent<NetworkObject>());
    }
    public override void OnNetworkDespawn()
    {
        //playerDied.Raise(this, GetComponentInParent<NetworkObject>());

        if (!IsOwner) return;
        DisableInput();    
     }

    private void DisableInput()
    {
        _input.Player.Disable();
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

    private void HandlePrimarySecondaryCheck(InputAction.CallbackContext obj)
    {
        // Button is held for LESS than drop time
        if (obj.duration < dropHoldTime)
        {
            if (obj.action == _input.Player.Primary && itemSlotManager.weaponInstance != null)
            {
                itemSlotManager.weaponInstance.GetComponent<ItemClass>().Use();
            }
            if (obj.action == _input.Player.Secondary && itemSlotManager.pickupInstance != null)
            {
                itemSlotManager.pickupInstance.GetComponent<ItemClass>().Use();
            }
        } 
        // Button is held for MORE than drop time
        else
        {
            if (obj.action == _input.Player.Primary && itemSlotManager.weaponInstance != null)
            {
                itemSlotManager.weaponInstance.GetComponent<ItemClass>().DropItemServerRpc("weapon", _throwItemInputHandler.transform.position);
            }
            if (obj.action == _input.Player.Secondary && itemSlotManager.pickupInstance != null)
            {
                itemSlotManager.pickupInstance.GetComponent<ItemClass>().DropItemServerRpc("pickup", _throwItemInputHandler.transform.position);
            }
        }        
    }


}
