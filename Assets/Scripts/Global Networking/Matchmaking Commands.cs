using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using System.Runtime.InteropServices;

/// <summary>
/// Global class that handles the essential Session commands (create a session, join a preexisting session, start a session).
/// Once connection process requires a more robust system, this can serve as a base.
/// </summary>
public class MatchmakingCommands : NetworkBehaviour
{
    public static MatchmakingCommands Instance { get; private set; }

    [SerializeField] private UnityTransport _transport;

    private void Start()
    {
        // Make global instance of all matchmaking commands
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);

        if (IsServer)
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
            NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        }
    }

    // Log player into Unity's Authentication Service with a unique anonymous one-time ID
    public async Task Login()
    {

        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            var options = new InitializationOptions();

            await UnityServices.InitializeAsync(options);
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }
    }


    // #### UNITY LOBBY+RELAY SERVICES ####
    [SerializeField] private GameEvent_SO onJoinLobby; // TEMPORARY WILL REMOVE LATER PROBABLY (just connected to MM_UIManager to display lobby code)

    public async Task<SessionData> CreateNewSession()
    {
        await Login();
        SessionData newSession = new();

        // Start Host
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            newSession.relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            _transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            newSession.errorStatus = e.ToString();
            return newSession;
        }


        // Start Lobby
        Lobby lobby;
        try
        {
            var options = new CreateLobbyOptions // Lobby Options object holds lobby data shared between clients
            {
                IsPrivate = true,
                Player = new Player(),
                Data = new Dictionary<string, DataObject>
                {
                    {"Relay Join Code", new DataObject(DataObject.VisibilityOptions.Member, newSession.relayJoinCode) },
                    {"Lobby Name", new DataObject(DataObject.VisibilityOptions.Public, "lobby") },
                    {"Player Count", new DataObject(DataObject.VisibilityOptions.Public, "4") },
                    {"Game Started", new DataObject(DataObject.VisibilityOptions.Member, "false") },
                    {"Lobby Is Alive", new DataObject(DataObject.VisibilityOptions.Public, "true")},
                    {"Map Choice", new DataObject(DataObject.VisibilityOptions.Public, "Default")}
                    // ----- Eventually should hold data for the variation and map choices for the session
                }
            };

            lobby = await Lobbies.Instance.CreateLobbyAsync(options.Data["Lobby Name"].Value, int.Parse(options.Data["Player Count"].Value), options);
            newSession.lobbyId = lobby.Id;

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            newSession.errorStatus = e.ToString();
            return newSession;
        }

        StartCoroutine(HandleLobbyHeartBeat(lobby)); // Always active until session ends
        HandleLobbyPoll(lobby);
        onJoinLobby.Raise(this, lobby);

        // Edit player list of session
        newSession.players.Add(new PlayerSessionData(NetworkManager.Singleton.LocalClientId, "Player"));

        // Initialize first state of server's current session data
        SyncSessionData.Instance.StartSessionDataSync_ServerRpc(NetworkManager.Singleton.LocalClientId);
        return newSession;

    }

    [SerializeField] GameObject sessionDataPrefab;

    public async Task<SessionData> JoinSession(string lobbyCode)
    {
        await Login();
        SessionData session = new();

        // Join Lobby
        Lobby lobby;
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = new Player()
            };
            lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            //PlayerManagernewSession.lobbyId = lobby.Id;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            session.errorStatus = e.ToString();
            return session;
        }
        // Join Relay
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobby.Data["Relay Join Code"].Value);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            _transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            session.errorStatus = e.ToString();
            return session;
        }

        // SUCCESS
        StartCoroutine("HandleLobbyPoll", lobby);
        onJoinLobby.Raise(this, lobby);

        return session;
    }

    public async void StartGameSession(SessionData session)
    {
        try
        {
            Lobby lobby = await Lobbies.Instance.GetLobbyAsync(session.lobbyId);
            lobby.Data["Game Started"] = new DataObject(DataObject.VisibilityOptions.Member, "true");
            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                Data = lobby.Data
            });
            // Now under HandleLobbyPollForUpdates(), the game will start. Probably should change just to do rpc calls
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            session.errorStatus = e.ToString();
        }
    }


    // Called whenever connection is being approved by relay
    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;
    }

    public static event Action<string> changeToScene;

    // Polling
    private async Task HandleLobbyPoll(Lobby lobby)
    {
        while (lobby != null)
        {
            lobby = await Lobbies.Instance.GetLobbyAsync(lobby.Id);
            if (lobby.Data["Game Started"].Value == "true")
            {
                // CREATE GAME & CHANGE SCENES ####################### DEFINITELY MODIFY THIS
                changeToScene?.Invoke(lobby.Data["Map Choice"].Value);

                // Stop polling for updates once game is joined
                // Later, restart polling when back in lobby menu
                return;
            } // else
            await Task.Delay(1000);
        }
        return;
    }
    private IEnumerator HandleLobbyHeartBeat(Lobby lobby)
    {
        while (lobby.Data["Lobby Is Alive"].Value == "true")
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobby.Id);
            yield return new WaitForSeconds(15.0f);
        }
    }

}
