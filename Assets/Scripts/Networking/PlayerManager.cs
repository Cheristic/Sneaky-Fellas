using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Dynamic;
using Unity.Networking;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }

    //Dictionary<ulong, GameObject> playerdict;

    //public NetworkVariable<Dictionary> PlayerData = new NetworkVariable<Dictionary>()<ulong, GameObject> { get => playerdict; set => playerdict = value; }

    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        Debug.Log("Starting Player Manager");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayersServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        SpawnPlayersClientRpc();
        
        //ulong objectId = playerInstance.GetComponent<NetworkObject>().NetworkObjectId;
        //PlayerData.Add(objectId, playerInstance);
        //InstantiatePlayersClientRpc(objectId);
    }

    [ClientRpc]
    private void SpawnPlayersClientRpc(ClientRpcParams clientRpcParams = default)
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        Debug.Log("Player Id = " + clientId);
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        NetworkPrefab playerNetworkPrefab = new NetworkPrefab();
        playerNetworkPrefab.Prefab = playerPrefab;

        NetworkManager.Singleton.GetComponent<NetworkManager>().NetworkConfig.Prefabs.Add(playerNetworkPrefab);

        newPlayer.SetActive(true);

        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

}
