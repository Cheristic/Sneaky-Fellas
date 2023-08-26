using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InGameManager : NetworkBehaviour
{
    public static InGameManager Instance { get; private set; }

    public ItemSpawnManager itemSpawnManager;

    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }

        if (IsServer)
        {
            PlayerSpawnManager.Instance.SpawnPlayersServerRpc();
            itemSpawnManager.SpawnItemsServerRpc();
        }
    }
}
