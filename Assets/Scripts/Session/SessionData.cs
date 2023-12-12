using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;

public class SessionData : NetworkBehaviour
{
    public Lobby lobby;
    [Header("Events")]
    public GameEvent onJoinLobby;
    public Matchmaking matchmaking;

    public SessionData()
    {
        matchmaking = GetComponentInChildren<Matchmaking>();
    }

    public async void StartSession()
    {
        await matchmaking.Login();
        lobby = await matchmaking.CreatePrivateLobby();
        if (lobby != null)
        {
            onJoinLobby.Raise(null, lobby);
            // Start polling to keep lobby alive and for updates
            if (IsServer) StartCoroutine(matchmaking.HandleLobbyHeartBeat(lobby));
            matchmaking.HandleLobbyPollForUpdates(lobby);
        }
    }

    public async void JoinSession(string lobbyCode)
    {
        // Weird quirk of passing strings through GameEvents is it adds a broken character to the end, this removes it
        lobbyCode = lobbyCode.Substring(0, lobbyCode.Length - 1);
        await matchmaking.Login();
        lobby = await matchmaking.JoinPrivateLobby(lobbyCode);
        if (lobby != null)
        {
            onJoinLobby.Raise(null, lobby);
            // Start polling for updates
            matchmaking.HandleLobbyPollForUpdates(lobby);
        }
    }

    public void StartGame()
    {
        if (lobby == null) return;

        matchmaking.StartGame(lobby);
        ProjectSceneManager.Instance.ChangeToMapScene();
    }
}
