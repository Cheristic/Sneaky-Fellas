using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MainPlayerInterface : MonoBehaviour
{
    [HideInInspector] public PlayerInterface playerInterface;
    // Keep track of all network player scripts here
    public MainPlayerHealth mainPlayerHealth;
    public FOVManager fovManager;
    public MainPlayerSyncSender mainPlayerSyncSender;
    public MainPlayerController mainPlayerController;
    private void Start()
    {
        playerInterface = GetComponentInParent<PlayerInterface>();
    }
}
