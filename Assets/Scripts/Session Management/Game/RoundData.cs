using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// Keeps track of player deaths and other temporary data. Resets each round.
/// </summary>
public class RoundData : INetworkSerializable
{
    // VARIABLES ONLY FOR ASSEMBLER (AKA SERVER)
    public List<GameObject> playersSpawned;
    public List<ulong> alivePlayers;

    public RoundData()
    {
        playersSpawned = new();
        alivePlayers = new();
        foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            alivePlayers.Add(client);
        }

    }

    public void NewRound()
    {
        alivePlayers.Clear();
        foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            alivePlayers.Add(client);
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {

    }

}
