using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Netcode;

/// <summary>
/// Handle countdown for beginning of each round
/// </summary>
public class RoundCountdown : NetworkBehaviour, INetworkUpdateSystem
{
    [SerializeField] Sprite[] countdownSprites;
    public static event Action StartRound;

    private NetworkVariable<bool> roundReady = new(false);
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SyncGameData.TriggerNewRoundReady.AddListener(StartCountdown); // Set round ready from server
            roundReady.Value = false;
        }
        
        NetworkUpdateLoop.RegisterNetworkUpdate(this, NetworkUpdateStage.Update); // All clients enter update loop to check roundReady's status in sync
        SyncGameData.BuildNewRound.AddListener(OnBuildNewRound);
    }

    private void OnBuildNewRound()
    {
        NetworkUpdateLoop.RegisterNetworkUpdate(this, NetworkUpdateStage.Update);
    }

    public void NetworkUpdate(NetworkUpdateStage net)
    {
        if (roundReady.Value == true) {
            StartCoroutine(CountdownEnumerator(3));
            NetworkUpdateLoop.UnregisterNetworkUpdate(this, NetworkUpdateStage.Update); // Remove to avoid update check
        }
    }

    // Set roundReady to true for all clients
    private void StartCountdown() => roundReady.Value = true;

    // Called once client detects roundReady is true
    private IEnumerator CountdownEnumerator(int total)
    {
        Image image = GetComponent<Image>();
        image.sprite = countdownSprites[2];
        while (true)
        {
            float c = 0;
            total--;
            
            // counter will add up until it reaches 1 seconds
            while (c < 1)
            {
                c += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, c);
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                yield return null;

            }
            // After 1 second..
            if (total == 0)
            {
                StartRound.Invoke(); // Start round
                yield break;
            }
            else
            {
                image.sprite = countdownSprites[total - 1]; // Change from 3->2->1
                yield return null;
            }
        }
    }
}
