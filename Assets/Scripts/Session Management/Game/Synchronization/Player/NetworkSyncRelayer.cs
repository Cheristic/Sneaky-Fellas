using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.Services.Lobbies.Models;
using static SyncNetworkPlayersDataManager;

public class NetworkSyncRelayer : MonoBehaviour
{
    RoundData roundData; // local reference
    public void Init()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("RelayMovementMessage", RelayMovementMessage);
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("RelayPlayerDies", RelayPlayerDies);
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("RelayMovementMessage");
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("RelayPlayerDies");
    }

    // Callback for sending player movement (Server only), send to all clients
    private void RelayMovementMessage(ulong senderId, FastBufferReader messagePayLoad)
    {
        byte[] recvData;
        messagePayLoad.ReadValueSafe(out recvData);
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(recvData), Allocator.Temp);
        var man = NetworkManager.Singleton.CustomMessagingManager;
        using (writer)
        {
            writer.WriteValueSafe(recvData);
            foreach (PlayerInterface player in InGameDataInterface.Instance.roundData.players)
            {
                man.SendNamedMessage("ReceiveMovementMessage"+senderId, player.playerId, writer, NetworkDelivery.Unreliable);
            }
        }
    }

    // Called by clients when they have passed
    public void RelayPlayerDies(ulong senderId, FastBufferReader messagePayLoad)
    {
        byte[] recvData;
        messagePayLoad.ReadValueSafe(out recvData);
        PlayerDiesMessage data = (PlayerDiesMessage)SerializationCommands.ToObject(recvData);
        // No need to verify request, if player verifies death on own client it'll seem fair
        InGameDataInterface.Instance.roundData.alivePlayers.Remove(data.playerId);

        byte[] msg = SerializationCommands.ToBytes(data);
        // Send death message to all clients
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(msg), Allocator.Temp);
        var man = NetworkManager.Singleton.CustomMessagingManager;
        using (writer)
        {
            writer.WriteValueSafe(msg);
            foreach (PlayerInterface player in InGameDataInterface.Instance.roundData.players)
            {
                man.SendNamedMessage("PlayerDies" + data.playerId, player.playerId, writer, NetworkDelivery.Unreliable);
            }
        }

        //if (GameInterface.Instance.roundData.alivePlayers.Count == 1)
        //  RoundEnd.Invoke(GameInterface.Instance.roundData.alivePlayers[0]); // Send winner client Id

    }
}
