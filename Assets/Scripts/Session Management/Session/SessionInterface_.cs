using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

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

    public async Task<bool> Host()
    {
        currentSession = await MatchmakingCommands.Instance.CreateNewSession();
        if (!String.IsNullOrEmpty(currentSession.errorStatus)) // ERROR HAS OCCURRED
        {
            // Handle error
            return false;
        }
        return true;
    }

    public async Task<bool> JoinPrivate(string lobbyCode)
    {
        currentSession = await MatchmakingCommands.Instance.JoinSession((string)lobbyCode); // return temp session because SyncSessionData will change currentSession
        if (!String.IsNullOrEmpty(currentSession.errorStatus)) // ERROR HAS OCCURRED
        {
            // Handle error
            return false;
        }
        return true;
    }

    public async void LeaveLobby()
    {
        await Task.Run(() => MatchmakingCommands.Instance.LeaveLobby(currentSession));
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
