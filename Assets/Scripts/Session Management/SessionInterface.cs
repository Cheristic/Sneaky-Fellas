using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

/// <summary>
/// Outermost layer for handling the basic Session inputs received through the Main Menu
/// </summary>
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
        currentSession = await MatchmakingCommands.Instance.JoinSession((string)lobbyCode); // return temp session because SyncSessionData will change currentSession
        if (!String.IsNullOrEmpty(currentSession.errorStatus)) // ERROR HAS OCCURRED
        {
            // Handle error
        }
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
