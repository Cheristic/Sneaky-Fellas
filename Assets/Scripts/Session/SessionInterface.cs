using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class SessionInterface : NetworkBehaviour
{
    public static SessionInterface Instance { get; private set; }

    public SessionData currentSession;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);

    }

    public async void Host()
    {
        currentSession = await MatchmakingCommands.Instance.CreateNewSession();
        if (!String.IsNullOrEmpty(currentSession.errorStatus)) // ERROR HAS OCCURRED
        {
            // Handle error
        }
    }

    public async void JoinPrivate(string lobbyCode)
    {
        SessionData session = await MatchmakingCommands.Instance.JoinSession((string)lobbyCode); // return temp session because SyncSessionData will change currentSession
        if (!String.IsNullOrEmpty(session.errorStatus)) // ERROR HAS OCCURRED
        {
            // Handle error
        }
        // Add to player list then through a ClientRpc call, client will receive the server's current session data
        SyncSessionData.Instance.AddPlayer_ServerRpc(NetworkManager.Singleton.LocalClientId, "Player");
    }

    public void StartGame()
    {
        if (currentSession != null)
        {
            MatchmakingCommands.Instance.StartGameSession(currentSession);
            if (!String.IsNullOrEmpty(currentSession.errorStatus)) // ERROR HAS OCCURRED
            {
                // Handle error
            }
        }
        
    }
}
