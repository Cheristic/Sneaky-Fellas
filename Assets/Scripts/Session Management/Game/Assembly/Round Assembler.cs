using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Soon as a Round starts, this prepares the game and tells the interface to begin the round.
/// Once variations come into play, this is what will be responsible for implementing them.
/// Basically serves as an abstract layer between Game Assembler and RoundData.
/// </summary>
public class RoundAssembler : MonoBehaviour
{
    private RoundData roundData; // Server/Assembler instance of round data

    private PlayerSpawner playerSpawner;
    private ItemSpawner itemSpawner;

    /// <summary>
    /// Causes Loading scene to unload and begins any timer (EVENTUALLY) for round to start.
    /// </summary>
    public static event Action TriggerStartFirstRound;
    public static event Action TriggerStartNewRound;

    private void Start()
    {
        playerSpawner = GameInterface.Instance.gameObject.AddComponent<PlayerSpawner>();
        //itemSpawner = new();
        FirstRound();
        SyncRoundData.RoundEnd += RoundEnd;
    }

    // ############# ROUND STARTERS #############
    private void FirstRound()
    {
        roundData = new();
        playerSpawner.FirstRound(ref roundData.playersSpawned);
        DistributeRoundData_ClientRpc(roundData); // Distribute server's Round Data
        TriggerStartFirstRound?.Invoke();
    }

    /// <summary>
    /// Called to begin all subsequent rounds.
    /// </summary>
    private void NewRound()
    {
        roundData.NewRound();
        playerSpawner.NewRound(ref roundData);
        DistributeRoundData_ClientRpc(roundData); // Distribute server's Round Data
        TriggerStartNewRound?.Invoke();
    }

    // ############# RPC + HELPER METHODS #############
    [ClientRpc]
    private static void DistributeRoundData_ClientRpc(RoundData roundData)
    {
        GameInterface.Instance.roundData = roundData;
    }

    private void RoundEnd(ulong winnerClientId) { StartCoroutine(RoundEndTimer(5)); }

    private IEnumerator RoundEndTimer(int timer)
    {
        Debug.Log("entering");
        while (timer > 0)
        {
            Debug.Log(timer);
            timer--;
            yield return new WaitForSeconds(1f);
        }

        NewRound(); 
        yield break;
    }
}
