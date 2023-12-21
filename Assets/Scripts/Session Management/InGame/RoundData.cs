using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Keeps track of player deaths and other temporary data. Resets each round.
/// </summary>
public class RoundData : INetworkSerializable
{
    public List<GameObject> playersSpawned;
    public string jsonPlayersSpawned;
    public RoundData()
    {
        
    }

    public void NewRound()
    {

    }

    public void EndRound()
    {
        playersSpawned.Clear();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        jsonPlayersSpawned = JsonCommands.GameObjectListToJson(playersSpawned);
        s.SerializeValue(ref jsonPlayersSpawned);
    }


    private void AddPlayerObject(GameObject player)
    {
        playersSpawned.Add(player);
    }

}
