using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;

public class Matchmaking : NetworkBehaviour
{

    public static int defaultMaxPlayers = 4;
    public static string defaultLobbyName = "Name";

    public const string KEY_START_GAME = "StartGame_RelayCode";
    public const string IN_LOBBY_CODE = "0";
    public const string RELAY_CODE = "JoinLobby_RelayCode";

    [SerializeField] RelayManager relayManager;

    // Polling
    public IEnumerator HandleLobbyHeartBeat(Lobby lobby)
    {
        while(true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobby.Id);
            yield return new WaitForSeconds(15.0f);
        }
    }
    public async void HandleLobbyPollForUpdates(Lobby lobby)
    {
        float lobbyPollingTimer = 1.0f;
        if (lobby != null)
        {
            lobbyPollingTimer -= Time.deltaTime;
            if (lobbyPollingTimer < 0f)
            {
                lobby = await Lobbies.Instance.GetLobbyAsync(lobby.Id);

                if (lobby.Data[KEY_START_GAME].Value != IN_LOBBY_CODE)
                {
                    // Stop polling for updates once game is joined
                    // Later, restart polling when back in lobby menu
                    lobby = null;
                }
                HandleLobbyPollForUpdates(lobby);
            }

        }
    }

    public static string PlayerId { get; private set; }
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

            PlayerId = AuthenticationService.Instance.PlayerId;
        }
    }
    public async Task<Lobby> CreatePrivateLobby()
    {
        try
        {
            string relayCode = await relayManager.CreateRelay();

            var options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = new Player(),
                Data = new Dictionary<string, DataObject>
                {
                    {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, IN_LOBBY_CODE) },
                    {RELAY_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                }
            };

            var lobby = await Lobbies.Instance.CreateLobbyAsync(defaultLobbyName, defaultMaxPlayers, options);

            return lobby;

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return null;
        }
    }


    public async Task<Lobby> JoinPrivateLobby(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = new Player()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            relayManager.JoinRelay(lobby.Data[RELAY_CODE].Value);
            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    // Not Used Yet -- for joining public lobbies
    public async void CheckForLobbies()
    {
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
                    //JoinLobbyByCode(lobby.LobbyCode);
                }
            }
            else
            {
                //CreateLobby();
            }
        } catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }


    private async void LeaveLobby(Lobby lobby)
    {
        try { 
            await LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId);
        } catch (LobbyServiceException e) {
            Debug.LogError(e);
        }
    }

    public async void StartGame(Lobby lobby)
    {
        try
        {
            lobby.Data[KEY_START_GAME] = lobby.Data[RELAY_CODE];
            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                Data = lobby.Data
            });
        }
        catch (LobbyServiceException e)
        {
        Debug.LogError(e);
        }
    }

}
