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

    public static void UpdatePlayerName(ref List<PlayerSessionData> players, ulong clientId, string playerName = null)
    {
        playerName = !string.IsNullOrEmpty(playerName) ? playerName : "Player";
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].LocalClientId == clientId)
            {
                players[i] = new PlayerSessionData(clientId, playerName);
                break;
            }
        }
        onPlayerDatabaseChange?.Invoke();
    }

}


