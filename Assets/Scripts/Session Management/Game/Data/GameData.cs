using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// When session begins, a GameData instance is created containing its general information (like variations, game mode, map, wins).
/// The data will be saved once game begins and used to assemble the game.
/// </summary>
public class GameData : INetworkSerializable
{
    public GameOptions gameOptions;

    public GameData()
    {
        gameOptions = new(); // Set the game options
    }

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref gameOptions);
    }
}
