using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerInterface : MonoBehaviour
{
    [HideInInspector] public PlayerInterface playerInterface;
    // Keep track of all network player scripts here
    public NetworkPlayerHealth networkPlayerHealth;
    public NetworkPlayerSyncReceiver networkPlayerSyncReceiver;
    public NetworkPlayerController networkPlayerController;
    private void Start()
    {
        playerInterface = GetComponentInParent<PlayerInterface>();
        networkPlayerController.Init();
        networkPlayerSyncReceiver.Init();
    }
}
