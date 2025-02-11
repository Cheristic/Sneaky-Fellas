using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

[Serializable]
public struct PlayerSessionData : INetworkSerializable, IEquatable<PlayerSessionData>
{
    public ulong LocalClientId;
    public FixedString64Bytes PlayerName;


    public PlayerSessionData(ulong clientId, string playerName)
    {
        LocalClientId = clientId;
        PlayerName = playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref LocalClientId);
        serializer.SerializeValue(ref PlayerName);

    }

    public bool Equals(PlayerSessionData other)
    {
        return LocalClientId == other.LocalClientId &&
            PlayerName == other.PlayerName;
    }
}

