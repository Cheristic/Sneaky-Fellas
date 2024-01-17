using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.Services.Lobbies.Models;

public class NetworkSyncRelayer : MonoBehaviour
{
    RoundData roundData; // local reference
    public void Init()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("RelayMovementMessage", RelayMovementMessage);
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("RelayMovementMessage");
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
}
