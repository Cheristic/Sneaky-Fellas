using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using static SyncNetworkPlayersDataManager;

/// <summary>
/// Keeps local instance of player's health and used when Damage events are triggered.
/// </summary>
public class MainPlayerHealth : PlayerHealth
{
    MainPlayerInterface mainPlayerInterface;
    private void Start()
    {
        SyncGameData.TriggerNewRoundReady.AddListener(NewRound);
        mainPlayerInterface = GetComponent<MainPlayerInterface>();
    }

    private void NewRound()
    {
        isDead = false;
    }

    private void TakeDamage(int dmg)
    {

    }

    public static event Action mainPlayerDied;
    public void Die()
    {
        mainPlayerDied.Invoke();

        PlayerInterface.Main.playerObject.layer = 13; // PlayerDead
        PlayerInterface.Main.playerSprite.gameObject.layer = 13;

        PlayerInterface.Main.playerObject.GetComponent<PolygonCollider2D>().enabled = false;
        isDead = true;

        // Send to server that this player died
        mainPlayerInterface.mainPlayerSyncSender.SendDeathMessage(mainPlayerInterface.playerInterface.playerId);
    }

    void Update()
    {
        // #### DEBUG ####
        if (Input.GetKeyDown(KeyCode.L)) // Die
        {
            Die();
        }
    }

    private void OnDestroy()
    {
        SyncGameData.TriggerNewRoundReady.RemoveListener(NewRound);
    }
}
