using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public ulong ClientId;

    public FixedString64Bytes PlayerName;


    public PlayerData(ulong clientId, string playerName)
    {
        ClientId = clientId;
        PlayerName = playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);

    }

    public bool Equals(PlayerData other)
    {
        return ClientId == other.ClientId &&
            PlayerName == other.PlayerName;
    }
}

