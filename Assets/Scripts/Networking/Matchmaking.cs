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
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Matchmaking : NetworkBehaviour
{
    public TextMeshProUGUI updateText;
    public Lobby joinedLobby;
    public MM_PlayerDisplay playerDisplay;
    //private RelayManager relayManager;

    private float heartbeatTimer;
    private float lobbyPollingTimer;

    

    public UnityEvent<GameObject> OnJoinLobby;
    public static event EventHandler OnJoinedLobbyUpdate;
    public static event EventHandler OnKickedFromLobby;

    private NetworkVariable<FixedString64Bytes> hostPlayerId = new NetworkVariable<FixedString64Bytes>("", NetworkVariableReadPermission.Everyone);

    public static Matchmaking Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
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

        //AuthenticationService.Instance.ClearSessionToken();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            PlayerId = AuthenticationService.Instance.PlayerId;
        Debug.Log("Signed in player = " + PlayerId);

        }
    }

    public int maxPlayers = 4;
    public string lobbyName = "Name";
    private string relayCode;

    public const string KEY_START_GAME = "StartGame_RelayCode";
    public const string RELAY_CODE = "JoinLobby_RelayCode";

    [SerializeField] TextMeshProUGUI LobbyCodeUIText;
    [SerializeField] Button StartGameButton;
    public async void CreateLobby()
    {
        updateText.text = "Creating Lobby...";

        try
        {
            var options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = new Player(),
                Data = new Dictionary<string, DataObject>
                {
                    {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") }                 
                }
            };

            var lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            LobbyCodeUIText.text = lobby.LobbyCode;

            joinedLobby = lobby;

            updateText.text = "Lobby created";

            relayCode = await RelayManager.Instance.CreateRelay();

            options.Data.Add(RELAY_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayCode));
            joinedLobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = options.Data
            });

            StartGameButton.gameObject.SetActive(true);

            //PlayerGameDatabase.Instance.AddPlayerToDatabase(NetworkManager.Singleton.LocalClientId, PlayerId);



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
            float lobbyPollingTimerMax = 1.1f;
            lobbyPollingTimer -= Time.deltaTime;
            if (lobbyPollingTimer < 0f)
            {
                lobbyPollingTimer = lobbyPollingTimerMax;
                joinedLobby = await Lobbies.Instance.GetLobbyAsync(joinedLobby.Id);

                if (joinedLobby.Data[KEY_START_GAME].Value != "0")
                {
                    if (PlayerId != hostPlayerId.Value)
                    {
                        //RelayManager.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);

                        updateText.text = "In game";

                    }
                    joinedLobby = null;

                }
            }

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
                Player = new Player()
            };
            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            LobbyCodeUIText.text = lobbyCode;
            updateText.text = "Lobby joined";
            OnJoinLobby?.Invoke(gameObject);
            RelayManager.Instance.JoinRelay(joinedLobby.Data[RELAY_CODE].Value);
            //PlayerGameDatabase.Instance.AddPlayerToDatabase(NetworkManager.Singleton.LocalClientId, PlayerId);
        } 
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            updateText.text = "Failed to join lobby.";
        }
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
                Debug.Log("Starting game...");

                //string relayCode = await RelayManager.Instance.CreateRelay();

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });

                joinedLobby = lobby;

                ProjectSceneManager.Instance.ChangeToMapScene();

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
