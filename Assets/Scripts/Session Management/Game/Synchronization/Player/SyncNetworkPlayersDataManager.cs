using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

/// <summary>
/// Contains all the data types used to sync data over the network
/// </summary>
public class SyncNetworkPlayersDataManager : MonoBehaviour
{
    public static SyncNetworkPlayersDataManager Main { get; private set; }
    
    private void Start()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        }
        else
        {
            Main = this;
        }
    }

    [Header("Global Movement Properties")]
    public static float playerMovementNetworkSendRate = 5; // = msgs/second

    // Adapted from https://www.youtube.com/watch?v=t23ekiaGYio

    // ####### HANDLE MOVEMENT #######
    [Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;
    }
    [Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }
    [Serializable]
    public struct PlayerMovementMessage
    {
        public ulong playerId;
        public SerializableVector3 pos;
        public SerializableQuaternion rot;
        public float timeToLerp;
    }
    // May just create one total network message to reduce network traffic:
    // Includes items held, weapon use, health. Would it be cheaper to send one big message or more smaller messages
}
