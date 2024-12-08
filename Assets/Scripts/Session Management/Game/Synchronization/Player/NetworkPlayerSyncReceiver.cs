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
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("PlayerDies" + i.playerInterface.playerId, ReceiveDeathMessage);
    }
    private void OnDestroy()
    {
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ReceiveMovementMessage" + i.playerInterface.playerId);
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("PlayerDies" + i.playerInterface.playerId);
    }

    // Every client receives message, updates the player object
    private void ReceiveMovementMessage(ulong senderId, FastBufferReader messagePayLoad)
    {
        byte[] movData;
        messagePayLoad.ReadValueSafe(out movData);
        PlayerMovementMessage msg = (PlayerMovementMessage)SerializationCommands.ToObject(movData);

        // If network player ID and local player ID don't match (ex: disconnect, clientIDs get swapped), stop until corrected
        if (msg.playerId != i.playerInterface.playerId) return;

        i.networkPlayerController.ReceiveMovementMessage(msg);
    }

    private void ReceiveDeathMessage(ulong senderId, FastBufferReader messagePayLoad)
    {
        byte[] deathData;
        messagePayLoad.ReadValueSafe(out deathData);
        PlayerDiesMessage msg = (PlayerDiesMessage)SerializationCommands.ToObject(deathData);

        // If network player ID and local player ID don't match (ex: disconnect, clientIDs get swapped), stop until corrected
        if (msg.playerId != i.playerInterface.playerId) return;

        i.networkPlayerHealth.ReceiveDeathMessage(msg);
    }
}