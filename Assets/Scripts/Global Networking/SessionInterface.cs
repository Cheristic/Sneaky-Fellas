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
        Debug.Log(currentSession.lobby.LobbyCode);
    }

    public async void JoinPrivate(string lobbyCode)
    {
        currentSession = await MatchmakingCommands.Instance.JoinSession((string)lobbyCode);
        if (!String.IsNullOrEmpty(currentSession.errorStatus)) // ERROR HAS OCCURRED
        {
            // Handle error
        }
        Debug.Log(currentSession.lobby.LobbyCode);
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
