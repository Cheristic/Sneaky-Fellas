using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Soon as a Round starts, this prepares the game and tells the interface to begin the round.
/// Once variations come into play, this is what will be responsible for implementing them.
/// Basically serves as an abstract layer between Game Assembler and RoundData.
/// </summary>
public class RoundAssembler : NetworkBehaviour
{
    private RoundData roundData; // Local assembler reference of round data

    private PlayerSpawner playerSpawner;
    private ItemSpawner itemSpawner;



    // Called by server module
    public void Init()
    {
        SyncRoundData.RoundEnd += RoundEnd;
        playerSpawner = GetComponent<PlayerSpawner>();
        SyncGameData.TriggerBuildFirstRound.AddListener(FirstRoundAssemble);
    }

    // ############# FIRST ROUND STARTS ############# 
    public void FirstRoundAssemble()
    {
        //itemSpawner = new();
        roundData = new();
        playerSpawner.FirstRound(ref roundData);
        SyncRoundData.Instance.UpdateRoundData(roundData); // Distribute server's Round Data
        SyncGameData.TriggerFirstRoundReady?.Invoke(); // Only initiated first round, cues the scene manager to unload Loading scene
        SyncGameData.TriggerNewRoundReady.Invoke(); // Calls RoundCountdown on all clients to begin
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

        OnTransitionToRoundStats();
        yield break;
    }

    /// <summary>
    /// Will show the current round stats (wins, kills, etc) for a few seconds, then transitions to new round builder
    /// </summary>
    private void OnTransitionToRoundStats()
    {
        SyncGameData.TransitionToRoundStats?.Invoke();
        // Handle stats (EVENTUALLY)
        NewRoundAssemble();
    }

    // ############# SUBSEQUENT ROUND STARTS ############# 
    private void NewRoundAssemble()
    {
        SyncGameData.BuildNewRound.Invoke();
        roundData.NewRound();
        playerSpawner.NewRound(ref roundData);
        SyncRoundData.Instance.UpdateRoundData(roundData); // Distribute server's Round Data
        SyncGameData.TriggerNewRoundReady.Invoke(); // Calls RoundCountdown on all clients to begin
    }


    public override void OnNetworkDespawn()
    {
        SyncRoundData.RoundEnd -= RoundEnd;
        SyncGameData.TriggerBuildFirstRound.RemoveListener(FirstRoundAssemble);
    }
}
