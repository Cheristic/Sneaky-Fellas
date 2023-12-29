using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

/// <summary>
/// Global class that is responsible for syncing up the session's connection data between all connected clients. 
/// Primarily accessed through ServerRpcs for the host client
/// </summary>
public class SyncSessionData : NetworkBehaviour
{
    public static SyncSessionData Instance { get; private set; }

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // In order to send clientRpc's to specific clients, this is a dictionary where client id (key) : all other client ids (value)
    public Dictionary<ulong, List<ulong>> clientIdParamDictionary = new();
    private void UpdateClientIdDict()
    {
        clientIdParamDictionary.Clear();
        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            clientIdParamDictionary.Add(id, new List<ulong>());
            //clientIdParamDictionary[id].Clear();
            foreach (ulong subid in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (subid != id)
                {
                    clientIdParamDictionary[id].Add(subid);
                }
            }
        }
    }

    // Alright, whenever a client joins or disconnects, we'll be updating a local array that contains the ids of all other clients except the local client

    // Initialized by host client
    [ServerRpc]
    public void StartSessionDataSync_ServerRpc(ulong clientId)
    {
        clientIdParamDictionary.Add(clientId, new List<ulong>());
        NetworkManager.Singleton.OnClientConnectedCallback += AddPlayer_ServerRpc;
        NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayer_ServerRpc;
    }
    /// <summary>
    /// Once joining client is connected, it can begin using RPCs.
    /// Add to player list and update current session data to all other clients.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void AddPlayer_ServerRpc(ulong clientId)
    {
        print($"id {NetworkManager.Singleton.LocalClientId}");
        SessionInterface.Instance.currentSession.players.Add(new PlayerSessionData(clientId, "Player"));

        clientIdParamDictionary.Add(clientId, new List<ulong>());
        UpdateClientIdDict();

        UpdateSessionData_ServerRpc(null, 0);
    }

    // Whenever player disconnects, update the session data
    [ServerRpc(RequireOwnership = false)]
    public void RemovePlayer_ServerRpc(ulong clientId) // BROKEN
    {
        print($"removing {clientId} from size {SessionInterface.Instance.currentSession.players.Count}");
        foreach (var item in clientIdParamDictionary.Keys)
        {
            print(item);
        }
        SessionInterface.Instance.currentSession.players.RemoveAt((int)clientId);

        List<ulong> l;
        bool success = clientIdParamDictionary.TryGetValue(clientId, out l);
        if (success) clientIdParamDictionary.Remove(clientId);
        UpdateClientIdDict();

        UpdateSessionData_ServerRpc(null, 0);
    }



    // Client will request server for data change and Server will distribute the updated version
    // DUAL RPCS - FIRST CALL SERVER
    [ServerRpc(RequireOwnership = false)]
    public void UpdateSessionData_ServerRpc(SessionData sd, ulong callingId)
    {
        if (sd != null) // null when client is joining or leaving, not just altering the contents of any particular session info
        {
            sd.players = JsonCommands.PlayerDataFromJson(sd.jsonPlayers);
            SessionInterface.Instance.currentSession = sd;
        }
        
        List<ulong> l = null;
        bool success = clientIdParamDictionary.TryGetValue(callingId, out l);
        
        if (success) // Send only to selected clients
        {
            ClientRpcParams c = new() { Send = new() { TargetClientIds = l } };
            UpdateSessionData_ClientRpc(SessionInterface.Instance.currentSession, c);
        }
    } // THEN CALL CLIENTS
    [ClientRpc]
    public void UpdateSessionData_ClientRpc(SessionData sd, ClientRpcParams p = default)
    {
        sd.players = JsonCommands.PlayerDataFromJson(sd.jsonPlayers);
        SessionInterface.Instance.currentSession = sd;

    }
}
