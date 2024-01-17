using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// Keeps track of player deaths and other temporary in-game data. Resets each round.
/// </summary>
public class RoundData : INetworkSerializable
{
    // VARIABLES ONLY FOR ASSEMBLER (AKA SERVER)
    public List<PlayerInterface> players;
    public List<ulong> alivePlayers;

    // ACCESSIBLE BY ALL
    public NetworkObjectReference[] playerNetworkObjects; // organized by playerId

    public RoundData()
    {
        players = new();
        playerNetworkObjects = new NetworkObjectReference[SessionInterface.Instance.currentSession.players.Count];
        alivePlayers = new();
        foreach (PlayerSessionData player in SessionInterface.Instance.currentSession.players)
        {
            alivePlayers.Add(player.LocalClientId);
        }
    }

    public void NewRound()
    {
        alivePlayers.Clear();
        foreach (PlayerSessionData player in SessionInterface.Instance.currentSession.players)
        {
            alivePlayers.Add(player.LocalClientId);
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref playerNetworkObjects);
    }

}
