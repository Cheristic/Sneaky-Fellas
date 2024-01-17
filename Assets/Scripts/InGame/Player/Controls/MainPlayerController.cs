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
    Transform playerObjectTransform;
    Rigidbody2D playerObjectRigidBody;
    private PlayerInputAction inputs;
    private ItemSlotManager itemSlotManager;
    private Interact_InputHandler interactor;

    [Header("Attributes")]
    [SerializeField] private float movementSpeed = 30.0f;
    [SerializeField] private float rotateSpeed = 3.0f;

    public static event Action mainPlayerCreated;

    void Start() // Called on spawn
    {
        playerObjectTransform = PlayerInterface.Main.playerObject.transform;
        playerObjectRigidBody = PlayerInterface.Main.playerObject.GetComponent<Rigidbody2D>();
        inputs = new();
        itemSlotManager = new(ref inputs);

        RoundCountdown.StartRound += OnStartRound;
        SyncGameData.TransitionToRoundStats.AddListener(OnTransitionToStats);

        mainPlayerCreated?.Invoke();
    }

    private Vector2 movementVector = new();
    public static event Action<Vector3> updateFOV;

    private void OnStartRound()
    {
        inputs.MainPlayer.Enable();
    }

    void OnTransitionToStats()
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
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(playerObjectTransform.position);
        var targetAngle = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        Quaternion currentAngle = playerObjectTransform.rotation;
        playerObjectTransform.rotation = Quaternion.Lerp(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);

        // Transmit direction to Vision Cones
        float finalAngle = playerObjectTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        var finalDir = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), dir.z);
        updateFOV?.Invoke(finalDir);
    }

    private void FixedUpdate()
    {
        playerObjectRigidBody.velocity = new Vector2(movementVector.x, movementVector.y).normalized * movementSpeed;
    }
}
