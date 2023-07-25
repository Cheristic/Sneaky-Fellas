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

    Dictionary<ulong, GameObject> playerdict;

    public Dictionary<ulong, GameObject> PlayerData { get => playerdict; set => playerdict = value; }

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

    public void InstatiatePlayers()
    {
        Debug.Log("Creating player0");
        var playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        Debug.Log("Creating player1");

        var playerId = NetworkManager.LocalClientId;
        Debug.Log("Creating player2" + playerId);

        PlayerData.Add(playerId, playerInstance);
        Debug.Log("Creating player3");

    }

    public void AddPlayers()
    {
        Debug.Log("hoi0");
        //var player = NetworkManager.ConnectedClients[playerId];


        //var playerInstance = Instantiate(player.PlayerObject, Vector3.zero, Quaternion.identity);
        //Debug.Log("hoi1 "  + player.ToString() + " " + playerID);
        //var playerId = NetworkManager.LocalClientId;
        //Debug.Log("hoi2 - " + playerId);

        if (!IsServer) return;
        Debug.Log("hoi3");

        //Instance.Players.Add(player.ClientId, playerInstance);
        ConnectClientsClientRpc();
        //player.SetActive(true);
        //NetworkManager.GetNetworkPrefabOverride(player);
        //player.GetComponent<NetworkObject>().CheckObjectVisibility = playerID => { return true; };
        //NetworkObject networkObject = player.GetComponent<NetworkObject>();
        //var playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        //NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

        //Instance.PlayerData.Add(playerId, player.PlayerObject);
        //playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId, true);
        //networkObject.SpawnAsPlayerObject(player, true);
        Debug.Log("hoi4");

    }
    [ClientRpc]
    private void ConnectClientsClientRpc()
    {
        if (IsOwner) return;

        var playerId = NetworkManager.LocalClientId;

        var playerInstance = Instance.PlayerData[playerId];
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId, true);

        /*var playerId = NetworkManager.LocalClientId;
        print("ID: " + playerId);
        var player = NetworkManager.ConnectedClients[playerId].PlayerObject;

        var playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        //NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

        //Instance.PlayerData.Add(playerId, player.PlayerObject);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId, true);*/
    }
}
