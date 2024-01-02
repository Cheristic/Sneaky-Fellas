using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Whenever a player wishes to update the round data (player dies, etc.), it'll access this scripts list of commands
/// </summary>
public class SyncRoundData : NetworkBehaviour
{
    public static SyncRoundData Instance { get; private set; }
    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [ClientRpc]
    private void UpdateRoundData_ClientRpc(RoundData roundData)
    {
        GameInterface.Instance.roundData = roundData;
    }

    public static event Action<ulong> RoundEnd; 

    // ########### COMMANDS #############

    [ServerRpc(RequireOwnership = false)]
    public void PlayerDies_ServerRpc(ulong clientId)
    {
        // No need to verify request, if player verifies death on own client it'll seem fair
        GameInterface.Instance.roundData.alivePlayers.Remove(clientId);
        PlayerDies_ClientRpc(clientId);
        //UpdateRoundData_ClientRpc(GameInterface.Instance.roundData);
        if (GameInterface.Instance.roundData.alivePlayers.Count == 1)
            RoundEnd.Invoke(GameInterface.Instance.roundData.alivePlayers[0]); // Send winner client Id
    }

    [ClientRpc]
    private void PlayerDies_ClientRpc(ulong clientId)
    {
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerInterface>().playerHealth.isDead = true;
    }

    // #### Debug ####
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && IsServer)
        {
            RoundEnd.Invoke(NetworkManager.Singleton.LocalClientId); // Send winner client Id
        }
    }
}
