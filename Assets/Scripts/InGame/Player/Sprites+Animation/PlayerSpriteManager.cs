using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Main hub for handling player's current sprite and animation. Disjointed from player object, follows it.
/// </summary>
public class PlayerSpriteManager : NetworkBehaviour
{
    private GameObject playerObject;
    private Rigidbody2D playerRB;
    private Animator movementAnimator;
    void Start()
    {
        if (IsOwner)
        {
            // EVENTS
            MainPlayerHealth.mainPlayerDied += OnDie;
            playerObject = PlayerInterface.Main.playerObject;
        } else
        {
            playerObject = GetComponentInParent<PlayerInterface>().playerObject;
        }
        playerRB = playerObject.GetComponent<Rigidbody2D>();
        movementAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerRB.velocity.magnitude > 0) movementAnimator.SetBool("IsWalking", true); // Determine if idle or walking
        else movementAnimator.SetBool("IsWalking", false);

        movementAnimator.SetFloat("Aim", playerRB.rotation / 360 + .5f); // Set direction

        transform.position = playerObject.transform.position;
        
    }

    public void OnDie()
    {
        movementAnimator.SetBool("IsAlive", false);
    }
}
