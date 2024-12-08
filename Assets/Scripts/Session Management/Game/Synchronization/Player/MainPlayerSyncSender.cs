using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.VisualScripting;
using UnityEngine;
using static SyncNetworkPlayersDataManager;

/// <summary>
/// Attached to main player's player object to send its local information across the network
/// </summary>
public class MainPlayerSyncSender : MonoBehaviour
{

    [Header("Local Movement Properties")]
    private bool canSendNetworkMovement = false;
    private float timeBetweenMovementStart;
    private float timeBetweenMovementEnd;

    private Transform playerObjectTransform;

    private void Start()
    {
        playerObjectTransform = PlayerInterface.Main.playerObject.transform;
    }


    void Update()
    {
        if (!canSendNetworkMovement)
        {
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        }
    }

    private IEnumerator StartNetworkSendCooldown()
    {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds(1 / playerMovementNetworkSendRate);
        SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        timeBetweenMovementEnd = Time.time;
        SendMovementMessage(PlayerInterface.Main.playerId, playerObjectTransform.position, playerObjectTransform.rotation, timeBetweenMovementEnd - timeBetweenMovementStart);
        canSendNetworkMovement = false;
    }

    // Send to server 'networkSendRate' times per second, server will relay to all clients
    public void SendMovementMessage(ulong _playerId, Vector3 _pos, Quaternion _rot, float _timeToLerp)
    {
        PlayerMovementMessage data = new PlayerMovementMessage()
        {
            playerId = _playerId,
            timeToLerp = _timeToLerp
        };
        data.pos.x = _pos.x; data.pos.y = _pos.y; data.pos.z = _pos.z;
        data.rot.x = _rot.x; data.rot.y = _rot.y; data.rot.z = _rot.z; data.rot.w = _rot.w;

        byte[] msg = SerializationCommands.ToBytes(data);
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(msg), Allocator.Temp);
        var man = NetworkManager.Singleton.CustomMessagingManager;

        using (writer)
        {
            writer.WriteValueSafe(msg);
            man.SendNamedMessage("RelayMovementMessage", NetworkManager.ServerClientId, writer, NetworkDelivery.Unreliable);
        }
    }

    public void SendDeathMessage(ulong _playerId)
    {
        PlayerDiesMessage data = new PlayerDiesMessage
        {
            playerId = _playerId,
            endGame = false
        };
        byte[] msg = SerializationCommands.ToBytes(data);
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(msg), Allocator.Temp);
        var man = NetworkManager.Singleton.CustomMessagingManager;

        using (writer)
        {
            writer.WriteValueSafe(msg);
            man.SendNamedMessage("RelayPlayerDies", NetworkManager.ServerClientId, writer, NetworkDelivery.Unreliable);
        }
    }
}