using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

public class PlayerGameDatabase : NetworkBehaviour
{
    public static PlayerGameDatabase Instance { get; private set; }

    [SerializeField] private GameObject playerDisplay;

    private NetworkList<PlayerData> players;


    private void Awake()
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
        players = new NetworkList<PlayerData>();

    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            players.OnListChanged += HandlePlayerGameListChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            /*foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }*/
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            players.OnListChanged -= HandlePlayerGameListChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        players.Add(new PlayerData(clientId, "Player"));
        Debug.Log("List size " + players.Count);
    }


    private void HandleClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == clientId)
            {
                players.RemoveAt(i);
                break;
            }
        }
    }

    public PlayerData GetPlayerData(ulong clientId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == clientId)
            {
                return players[i];
            }
        }
        return players[0];
    }

    public PlayerData GetPlayerDataByIndex(int index)
    {
        return players[index];
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerNameServerRpc(ulong clientId, string playerName = null)
    {
        playerName = !string.IsNullOrEmpty(playerName) ? playerName : "Player";
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == clientId)
            {
                players[i] = new PlayerData(clientId, playerName);
                break;
            }
        }
    }


    public void HandlePlayerGameListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        if (Matchmaking.Instance.joinedLobby != null) Matchmaking.Instance.playerDisplay.UpdatePlayerDisplay();
    }

}
