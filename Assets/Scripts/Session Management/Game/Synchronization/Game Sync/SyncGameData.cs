using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Syncs all the Game Data but also includes the Round Management Events
/// </summary>
public class SyncGameData : NetworkBehaviour
{
    public static SyncGameData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Prepare management events
        TriggerBuildFirstRound = new UnityEvent();
        TriggerFirstRoundReady = new UnityEvent();
        TransitionToRoundStats = new UnityEvent();
        BuildNewRound = new UnityEvent();
        TriggerNewRoundReady = new UnityEvent();
    }

    // #### ROUND MANAGEMENT EVENTS (Only server will invoke)
    public static UnityEvent TriggerBuildFirstRound; // Called by ProjectSceneManager once all clients are in.
    public static UnityEvent TriggerFirstRoundReady; // Called first round. Unloads Loading scene and all inital processes
    public static UnityEvent TransitionToRoundStats; // Called once round ends to signal transition to stats
    public static UnityEvent BuildNewRound; // Called on all subsequent rounds to indicate a new round
    public static UnityEvent TriggerNewRoundReady; // Called every round after round is built to signal the countdown for all clients

    [ServerRpc]
    private void UpdateGameData_ServerRpc(GameData gameData)
    {
        
    }

    [ClientRpc]
    private void UpdateGameData_ClientRpc(GameData gameData)
    {
        InGameDataInterface.Instance.gameData = gameData;
    }
}
