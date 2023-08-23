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

    [SerializeField] private KnifeWeaponParent weaponParent;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        transform.position = new Vector3(Random.Range(5f, -5f), 0, Random.Range(5f, -5f));
        PlayerSpawnManager.Instance.OnGameStarted?.Invoke();

    }
    void Update()
    {
        if (!IsOwner)
        {
            gameObject.layer = LayerMask.NameToLayer("BehindMask");
            return;
        }
        HandleMovement();
        HandleRotation();
        HandleAttack();
    }

    private void HandleMovement()
    {

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        fieldOfView.SetOrigin(transform.position);
        fovCircle.SetOrigin(transform.position);
    }

    private void HandleRotation()
    {
        //Mouse rotation
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var targetAngle = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        Quaternion currentAngle = transform.rotation;
        transform.rotation = Quaternion.Lerp(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
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
            weaponParent.PlayerAttack();
        }
    }
}
