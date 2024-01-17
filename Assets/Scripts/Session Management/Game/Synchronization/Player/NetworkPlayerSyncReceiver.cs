using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static SyncNetworkPlayersDataManager;

/// <summary>
/// Attached to network sync object and will update the corresponding network player's object with info
/// </summary>
public class NetworkPlayerSyncReceiver : MonoBehaviour
{
    NetworkPlayerInterface i;
    public void Init()
    {
        i = GetComponent<NetworkPlayerInterface>();
        // REGISTER MESSAGES
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ReceiveMovementMessage"+i.playerInterface.playerId, ReceiveMovementMessage);
        
    }
    private void OnDestroy()
    {
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ReceiveMovementMessage" + i.playerInterface.playerId);
    }

    // Every client receives message, updates the player object
    private void ReceiveMovementMessage(ulong senderId, FastBufferReader messagePayLoad)
    {
        byte[] recvData;
        messagePayLoad.ReadValueSafe(out recvData);
        PlayerMovementMessage msg = (PlayerMovementMessage)SerializationCommands.ToObject(recvData);

        // If network player ID and local player ID don't match (ex: disconnect, clientIDs get swapped), stop until corrected
        if (msg.playerId != i.playerInterface.playerId) return;

        i.networkPlayerController.ReceiveMovementMessage(msg);
    }
}