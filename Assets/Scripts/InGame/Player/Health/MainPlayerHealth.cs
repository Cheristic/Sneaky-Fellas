using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Keeps local instance of player's health and used when Damage events are triggered.
/// </summary>
public class MainPlayerHealth : PlayerHealth
{
    private void TakeDamage(int dmg)
    {

    }

    public static event Action mainPlayerDied;
    public override void Die()
    {
        mainPlayerDied.Invoke();

        PlayerInterface.Main.playerObject.layer = 13; // PlayerDead
        PlayerInterface.Main.playerSprite.gameObject.layer = 13;

        PlayerInterface.Main.playerObject.GetComponent<PolygonCollider2D>().enabled = false;
        isDead = true;
    }

    void Update()
    {
        // #### DEBUG ####
        if (Input.GetKeyDown(KeyCode.L)) // Die
        {
            Die();
        }
    }
}
