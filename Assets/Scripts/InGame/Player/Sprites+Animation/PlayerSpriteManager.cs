using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main hub for handling player's current sprite and animation. Disjointed from player object, follows it.
/// </summary>
public class PlayerSpriteManager : MonoBehaviour
{
    private GameObject playerObject;
    private Rigidbody2D playerRB;
    private Animator movementAnimator;
    void Start()
    {
        playerObject = PlayerInterface.Main.playerObject;
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
}
