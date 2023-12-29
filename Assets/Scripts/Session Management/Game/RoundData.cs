using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Keeps track of player deaths and other temporary data. Resets each round.
/// </summary>
public class RoundData : INetworkSerializable
{
    // VARIABLES ONLY FOR ASSEMBLER
    public List<GameObject> playersSpawned; 

    public RoundData()
    {
        playersSpawned = new();
    }

    public void EndRound()
    {
        playersSpawned.Clear();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {

    }

}
