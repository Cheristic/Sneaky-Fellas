using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using Unity.VisualScripting;
using Newtonsoft.Json;

[System.Serializable]
public class SessionData : INetworkSerializable
{
    public string lobbyId;
    public string errorStatus; // Should remain empty, if not, then error has occurred
    public string relayJoinCode;
    public List<PlayerSessionData> players;
    public string jsonPlayers; // Used to conver the players list to json string across rpc calls
    public GameOptions gameOptions;


    public SessionData()
    {
        players = new();
        gameOptions = new();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref lobbyId);
        s.SerializeValue(ref relayJoinCode);
        jsonPlayers = JsonCommands.PlayerDataToJson(players);
        s.SerializeValue(ref jsonPlayers);
        s.SerializeValue(ref gameOptions);

    }
}
