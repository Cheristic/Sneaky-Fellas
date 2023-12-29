using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SyncRoundData : MonoBehaviour
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

    [ServerRpc(RequireOwnership = false)]
    private void UpdateRoundData_ServerRpc(RoundData roundData)
    {
        
    }

    [ClientRpc]
    private void UpdateRoundData_ClientRpc(RoundData roundData)
    {
        GameInterface.Instance.roundData = roundData;
    }
}
