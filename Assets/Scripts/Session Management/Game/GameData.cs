using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// When game begins, a GameData instance is created containing its general information (like variations, game mode, rounds won by each player).
/// </summary>
public class GameData : INetworkSerializable
{

    public GameOptions gameOptions;

    public GameData()
    {
        gameOptions = SessionInterface.Instance.currentSession.gameOptions; // Set the game options
    }

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref gameOptions);
    }
}
