using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering.Universal;

public class PlayerCameraInterface : NetworkBehaviour
{
    public static PlayerCameraInterface Main { get; private set; }

    public PlayerCameraMovement playerCameraMovement;

    private UniversalAdditionalCameraData additionalCameraData;

    public override void OnNetworkSpawn()
    {
        Main = this;
        SyncGameData.BuildNewRound.AddListener(SwitchToAlive);
        MainPlayerHealth.mainPlayerDied += SwitchToSpectate;
        additionalCameraData = GetComponent<UniversalAdditionalCameraData>();

    }

    private void SwitchToSpectate()
    {
        additionalCameraData.SetRenderer(1);
    }

    private void SwitchToAlive()
    {
        additionalCameraData.SetRenderer(0);
    }
}
