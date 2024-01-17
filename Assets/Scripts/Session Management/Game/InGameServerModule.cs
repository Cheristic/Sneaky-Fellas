using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Holds all game scripts only held by the server. Initializes the scripts
/// </summary>
public class InGameServerModule : NetworkBehaviour
{
    [SerializeField] NetworkSyncRelayer networkSyncRelayer;
    [SerializeField] RoundAssembler roundAssembler;
    [SerializeField] PlayerSpawner playerSpawner;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            Destroy(gameObject);
            return;
        }

        networkSyncRelayer.Init();
        roundAssembler.Init();
        playerSpawner.Init();
    }
}
