using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;
using Unity.Services.Lobbies.Models;

public class PlayerSessionDataCommands
{

    public static event Action onPlayerDatabaseChange;

    /*
    private void HandleClientConnected(ulong clientId)
    {
        players.Add(new PlayerSessionData(clientId, "Player"));
        onPlayerDatabaseChange?.Invoke();
    }


    private void HandleClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == clientId)
            {
                players.RemoveAt(i);
                break;
            }
        }
        onPlayerDatabaseChange?.Invoke();
    }

    public PlayerSessionData GetPlayerData(ulong clientId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == clientId)
            {
                return players[i];
            }
        }
        return players[0];
    }

    public PlayerSessionData GetPlayerDataByIndex(int index)
    {
        return players[index];
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }*/

    public static void UpdatePlayerName(ref List<PlayerSessionData> players, ulong clientId, string playerName = null)
    {
        playerName = !string.IsNullOrEmpty(playerName) ? playerName : "Player";
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == clientId)
            {
                players[i] = new PlayerSessionData(clientId, playerName);
                break;
            }
        }
        onPlayerDatabaseChange?.Invoke();
    }

}


