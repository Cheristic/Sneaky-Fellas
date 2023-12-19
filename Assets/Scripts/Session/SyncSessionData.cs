using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class SyncSessionData : NetworkBehaviour
{
    public static SyncSessionData Instance { get; private set; }

    public SessionData serverSessionData;

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
        if (IsServer) NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayer_ServerRpc;
    }

    // In order to send clientRpc's to specific clients, this is a dictionary where client id (key) : all other client ids (value)
    public Dictionary<ulong, List<ulong>> clientIdParamDictionary = new();
    private void UpdateClientIdDict()
    {
        clientIdParamDictionary.Clear();
        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            clientIdParamDictionary[id].Clear();
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
    public void StartSessionDataSync_ServerRpc(SessionData sd, ulong clientId)
    {
        sd.players = JsonCommands.PlayerDataFromJson(sd.jsonPlayers);
        serverSessionData = sd;
        clientIdParamDictionary.Add(clientId, new List<ulong>());
    }

    // Called by clients when joined, return current session data
    [ServerRpc]
    public void AddPlayer_ServerRpc(ulong clientId, string playerName)
    {
        serverSessionData.players.Add(new PlayerSessionData(clientId, playerName));

        clientIdParamDictionary.Add(clientId, new List<ulong>());
        UpdateClientIdDict();

        print("yo");
        UpdateSessionData_ServerRpc(serverSessionData);
    }

    // Whenever player disconnects, update the session data
    [ServerRpc]
    public void RemovePlayer_ServerRpc(ulong clientId)
    {
        serverSessionData.players.RemoveAt((int)clientId);

        clientIdParamDictionary.Remove(clientId);
        UpdateClientIdDict();

        UpdateSessionData_ServerRpc(serverSessionData);
    }


    // Client will edit their local session data then send it to the server to be distributed to the other clients
    // FIRST CALL SERVER
    [ServerRpc]
    public void UpdateSessionData_ServerRpc(SessionData sd, ulong callingId = 1024)
    {
        sd.players = JsonCommands.PlayerDataFromJson(sd.jsonPlayers);
        serverSessionData = sd;
        print("um");
        List<ulong> l = null;
        bool success = clientIdParamDictionary.TryGetValue(callingId, out l);
        
        if (success) // Send only to selected clients
        {
            ClientRpcParams c = new() { Send = new() { TargetClientIds = l } };
            UpdateSessionData_ClientRpc(serverSessionData, c);
        } else // Send to all clients
        {
            UpdateSessionData_ClientRpc(serverSessionData);
        }
        serverSessionData.players.ForEach(p => print(p.PlayerName));
    } // THEN IT'LL CALL CLIENTS
    [ClientRpc]
    public void UpdateSessionData_ClientRpc(SessionData sd, ClientRpcParams p = default)
    {
        sd.players = JsonCommands.PlayerDataFromJson(sd.jsonPlayers);
        SessionInterface.Instance.currentSession = sd;
    }
}
