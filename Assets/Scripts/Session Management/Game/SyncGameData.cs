using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SyncGameData : NetworkBehaviour
{
    public static SyncGameData Instance { get; private set; }
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

    [ServerRpc]
    private void UpdateGameData_ServerRpc(GameData gameData)
    {
        //
    }

    [ClientRpc]
    private void UpdateGameData_ClientRpc(GameData gameData)
    {
        GameInterface.Instance.gameData = gameData;
    }
}
