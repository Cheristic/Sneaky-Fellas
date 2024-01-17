using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using TMPro;
using Unity.Netcode;

public class GameUIManager : NetworkBehaviour
{
    [SerializeField] GameObject BlackFOVFilter;

    public override void OnNetworkSpawn()
    {
        SyncGameData.BuildNewRound.AddListener(OnBuildNewRound);
        MainPlayerHealth.mainPlayerDied += OnMainPlayerDied;
    }

    private void OnMainPlayerDied()
    {
        BlackFOVFilter.SetActive(false);
    }

    private void OnBuildNewRound()
    {
        BlackFOVFilter.SetActive(true);
    }
}