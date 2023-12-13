using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GameData : NetworkBehaviour
{
    [SerializeField] ItemSpawnManager itemSpawnManager;
    [SerializeField] SoundManager soundManager;
    [SerializeField] PlayerSpawnManager playerSpawnManager;

    public void GameStarting()
    {
        if (!IsServer) return;
        itemSpawnManager.SpawnItemsServerRpc();
        playerSpawnManager.SpawnPlayersServerRpc();
    }

    public void RestartRound()
    {
        itemSpawnManager.RespawnItemsServerRpc();
        playerSpawnManager.RespawnPlayersServerRpc();
    }

    public void AddPlayer()
    {

    }

}
