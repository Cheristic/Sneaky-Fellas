using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{

    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float rotateSpeed = 3.0f;
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private FieldOfView fovCircle;

    private float horizontal, vertical;
    private float diagLimiter = 0.7f;
    private Rigidbody2D rb;

    private ItemSlotManager itemSlotManager;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        itemSlotManager = GetComponent<ItemSlotManager>();

        if (!IsOwner)
        {
            gameObject.layer = LayerMask.NameToLayer("BehindMask");
            return;
        }
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
        HandleAttack();
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
        //Move player
        if (horizontal != 0 && vertical != 0)
        {
            horizontal *= diagLimiter;
            vertical *= diagLimiter;
        }
        rb.velocity = new Vector2(horizontal * playerSpeed, vertical * playerSpeed);
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (itemSlotManager.weaponInstance != null) itemSlotManager.weaponInstance.GetComponent<ItemClass>().Use();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (itemSlotManager.pickupInstance != null) itemSlotManager.pickupInstance.GetComponent<ItemClass>().Use();
        }
    }
}
