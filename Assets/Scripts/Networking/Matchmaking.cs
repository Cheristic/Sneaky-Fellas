using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;
using TMPro;
using Object = UnityEngine.Object;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Services.Relay.Models;

public class Matchmaking : NetworkBehaviour
{
    public TextMeshProUGUI updateText;
    private string playerName = null;
    private Lobby joinedLobby;
    //private RelayManager relayManager;

    private float heartbeatTimer;
    private float lobbyPollingTimer;

    public static event EventHandler OnGameStarted;
    public static event EventHandler OnJoinedLobbyUpdate;
    public static event EventHandler OnKickedFromLobby;

    private NetworkVariable<string> hostPlayerId = new NetworkVariable<string>(null, NetworkVariableReadPermission.Everyone);


    private void Start()
    {
        
    }

    private void Update()
    {
        HandleLobbyHeartBeat();
        HandleLobbyPollForUpdates();
    }

    public async void Host()
    {
        updateText.text = "Loggin in...";

        await Login();

        hostPlayerId.Value = PlayerId;

        CreateLobby();
    }


    public async void Play()
    {
        updateText.text = "Loggin in...";

        await Login();

        //CreateLobby();
        CheckForLobbies();

    }

    public async void PlayPrivate(string lobbyCode)
    {
        updateText.text = "Loggin in...";

        await Login();

        JoinLobbyByCode(lobbyCode);
    }

    public static string PlayerId { get; private set; }
    public static async Task Login()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            var options = new InitializationOptions();

            await UnityServices.InitializeAsync(options);
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            PlayerId = AuthenticationService.Instance.PlayerId;


        }
    }

    public int maxPlayers = 4;
    public string lobbyName = "Name";

    public const string KEY_START_GAME = "StartGame_RelayCode";


    public async void CreateLobby()
    {
        updateText.text = "Creating Lobby...";

        try
        {
            var options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };

            var lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            joinedLobby = lobby;

            updateText.text = "I am lobby host - " + lobby.LobbyCode;
        } 
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            updateText.text = "Failed to create lobby.";
        }
    }

    private async void HandleLobbyHeartBeat()
    {
        if (joinedLobby != null && IsServer) {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;
                await Lobbies.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                Debug.Log("Beat.");
            }

        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyPollingTimer -= Time.deltaTime;
            if (lobbyPollingTimer < 0f)
            {
                float lobbyPollingTimerMax = 1.1f;
                lobbyPollingTimer = lobbyPollingTimerMax;
                joinedLobby = await Lobbies.Instance.GetLobbyAsync(joinedLobby.Id);

                Debug.Log("Client joining relay3.");


                if (joinedLobby.Data[KEY_START_GAME].Value != "0")
                {
                    Debug.Log("Client joining relay2.");

                    if (PlayerId != hostPlayerId.Value)
                    {
                        Debug.Log("Client joining relay1. - " + PlayerId);
                        RelayManager.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                        updateText.text = "In game";
                        if (IsClient) { PlayerManager.Instance.InstantiatePlayers(); }

                    }
                    

                    joinedLobby = null;

                }
            }

        }
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        try { 
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                        {
                            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                        }
            });
        } catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            updateText.text = "Failed to find lobbies.";
        }
    }

    public async void CheckForLobbies()
    {
        updateText.text = "Finding lobby...";

        try { 

            var queryOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0"
                        )
                },
            };

            var response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
            var lobbies = response.Results;

            if (lobbies.Count > 0)
            {
                foreach (var lobby in lobbies)
                {
                    JoinLobbyByCode(lobby.LobbyCode);
                }
            }
            else
            {
                CreateLobby();
            }
        } catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            updateText.text = "Failed to find lobbies.";
        }
    }


    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            updateText.text = "Joined lobby with code: " + lobbyCode;  
        } 
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            updateText.text = "Failed to join lobby.";
        }
    }
    

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
        };
    }

    private async void LeaveLobby()
    {
        try { 
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        } catch (LobbyServiceException e) {
            Debug.LogError(e);
            updateText.text = "Failed to find lobbies.";
        }
    }

    public async void StartGame()
    {
        if (PlayerId == hostPlayerId.Value)
        {
            try
            {
                updateText.text = "Starting game...";

                string relayCode = await RelayManager.Instance.CreateRelay();

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });


                if (IsClient) { PlayerManager.Instance.InstantiatePlayers(); }

                joinedLobby = lobby;

                updateText.text = "In game";

            }
            catch (LobbyServiceException e)
            {
            Debug.LogError(e);
            updateText.text = "Failed to start game.";
            }
        }
    }
}
