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
    Transform pTransform;
    Rigidbody2D pRigidbody;
    private PlayerInputAction inputs;
    private ItemSlotManager itemSlotManager;
    private Interact_InputHandler interactor;

    [Header("Attributes")]
    [SerializeField] private float movementSpeed = 30.0f;
    [SerializeField] private float rotateSpeed = 3.0f;

    void Start()
    {
        pTransform = PlayerInterface.Main.playerObject.transform;
        pRigidbody = PlayerInterface.Main.playerObject.GetComponent<Rigidbody2D>();
        inputs = new();
        RoundCountdown.countdownOver += StartRound;
        itemSlotManager = new(ref inputs);
    }

    private Vector2 movementVector = new();
    public static event Action<Vector3> updateFOV;

    private void StartRound()
    {
        inputs.MainPlayer.Enable();
    }

    void OnDisable()
    {
        inputs.MainPlayer.Disable();
    }

    private void Update()
    {
        // #### MOVEMENT ####
        if(inputs.MainPlayer.enabled)
        {
            movementVector = inputs.MainPlayer.Move.ReadValue<Vector2>();
            //movementVector.x = Input.GetAxisRaw("Horizontal");
            //movementVector.y = Input.GetAxisRaw("Vertical");
        }
        
        // #### ROTATION ####
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(pTransform.position);
        var targetAngle = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        Quaternion currentAngle = pTransform.rotation;
        pTransform.rotation = Quaternion.Lerp(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);

        // Transmit direction to FOVs
        float finalAngle = pTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        var finalDir = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), dir.z);
        updateFOV.Invoke(finalDir);
    }

    private void FixedUpdate()
    {
        pRigidbody.velocity = new Vector2(movementVector.x, movementVector.y).normalized * movementSpeed;
    }
}
