using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UIElements;
/// <summary>
/// Whenever a player wishes to update the round data (player dies, etc.), it'll access this scripts list of commands
/// </summary>
public class SyncRoundData : NetworkBehaviour
{
    public static SyncRoundData Instance { get; private set; }

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
    }

    public void UpdateRoundData(RoundData roundData)
    {
        InGameDataInterface.Instance.roundData = roundData; // Update server's round data
        UpdateRoundData_ClientRpc(roundData);
    }

    [ClientRpc]
    private void UpdateRoundData_ClientRpc(RoundData roundData)
    {
        if (IsServer) return; // Don't update server's roundData -- it contains private info
        InGameDataInterface.Instance.roundData = roundData;
    }

    public static event Action<ulong> RoundEnd; 

    // ########### COMMANDS #############

    [ServerRpc(RequireOwnership = false)]
    public void PlayerDies_ServerRpc(ulong clientId)
    {
        // No need to verify request, if player verifies death on own client it'll seem fair
        InGameDataInterface.Instance.roundData.alivePlayers.Remove(clientId);

        ClientRpcParams c = new() { Send = new() { TargetClientIds = SyncSessionData.Instance.clientIdParamDictionary[clientId] } };
        PlayerDies_ClientRpc(clientId, c);
        //if (GameInterface.Instance.roundData.alivePlayers.Count == 1)
          //  RoundEnd.Invoke(GameInterface.Instance.roundData.alivePlayers[0]); // Send winner client Id
    
    }

    [ClientRpc]
    private void PlayerDies_ClientRpc(ulong clientId, ClientRpcParams c = default)
    {
        //NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerInterface>().playerHealth.Die();
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
