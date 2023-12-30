using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Added to main player object. Top level hub for controls. Handles movement and rotation.
/// </summary>
public class MainPlayerController : MonoBehaviour
{
    // When sending the player netcode, send its transform and the force applied to the rigidbody.
    Rigidbody2D rb;
    private PlayerInputAction inputs;
    private ItemSlotManager itemSlotManager;
    private Interact_InputHandler interactor;

    [Header("Attributes")]
    [SerializeField] private float movementSpeed = 30.0f;
    [SerializeField] private float rotateSpeed = 3.0f;

    void Start()
    {
        rb = PlayerInterface.Main.playerObject.GetComponent<Rigidbody2D>();
        inputs = new();
        inputs.MainPlayer.Enable();
        itemSlotManager = new(ref inputs);
    }

    private Vector2 movementVector = new();
    public static event Action<Vector3> updateFOV;

    private void Update()
    {
        // #### MOVEMENT ####
        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.y = Input.GetAxisRaw("Vertical");

        // #### ROTATION ####
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var targetAngle = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        Quaternion currentAngle = transform.rotation;
        transform.rotation = Quaternion.Lerp(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);

        // Transmit direction to FOVs
        float finalAngle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        var finalDir = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), dir.z);
        updateFOV.Invoke(finalDir);
    }

    private void FixedUpdate()
    {
        rb.velocity = movementVector.normalized * movementSpeed;
    }
}
