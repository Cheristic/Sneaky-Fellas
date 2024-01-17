using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameOptions : INetworkSerializable
{
    public MapChoice map;

    public GameOptions()
    {
        map = MapChoice.Default;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref map);
    }
}

public enum MapChoice
{
    Default,
    Erm
}
