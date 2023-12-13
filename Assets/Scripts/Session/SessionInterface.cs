using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SessionInterface : NetworkBehaviour
{
    SessionData currentSession;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void CreateNewSession()
    {
        currentSession = new SessionData(); 
    }

    public void Host()
    {
        CreateNewSession();
        currentSession.StartSession();
    }

    public void JoinPrivate(Component sender, object lobbyCode)
    {
        currentSession = GetComponentInChildren<SessionData>();
        currentSession.JoinSession((string)lobbyCode);
    }

    public void StartGame()
    {
        currentSession.StartGame();
    }
}
