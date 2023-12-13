using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

public class PlayerSessionDatabase : NetworkBehaviour
{
    public static PlayerSessionDatabase Instance { get; private set; }

    private NetworkList<PlayerSessionData> players;

    [SerializeField] private GameEvent onPlayerDatabaseChange;


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
        players = new NetworkList<PlayerSessionData>();

    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        players.Add(new PlayerSessionData(clientId, "Player"));
        onPlayerDatabaseChange.Raise(this, null);
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
        onPlayerDatabaseChange.Raise(this, null);
    }

    public PlayerSessionData GetPlayerData(ulong clientId)
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

    public PlayerSessionData GetPlayerDataByIndex(int index)
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
                players[i] = new PlayerSessionData(clientId, playerName);
                break;
            }
        }
        onPlayerDatabaseChange.Raise(this, null);
    }

}
