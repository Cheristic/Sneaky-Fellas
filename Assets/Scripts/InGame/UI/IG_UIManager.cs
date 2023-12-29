using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using TMPro;
using Unity.Netcode;

public class IG_UIManager : NetworkBehaviour
{

    private float pingTimer = 1;

    private void Start()
    {
    }
    public void OnHitRespawnButton()
    {
        
    }



    [SerializeField] private TextMeshProUGUI pingText;

    // PING STUFF WILL CHANGE
    private void Update()
    {
        if (IsHost)
        {
            pingTimer -= Time.deltaTime;
            if (pingTimer < 0f)
            {
                float pingTimerMax = 1;
                pingTimer = pingTimerMax;

                float ping = Time.realtimeSinceStartup;
                GetPingServerRpc(ping);
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void GetPingServerRpc(float ping)
    {
        GetPingClientRpc(ping);
    }

    [ClientRpc]
    private void GetPingClientRpc(float ping)
    {
        ping = (Time.realtimeSinceStartup - ping) * 1000;
        pingText.text = "Ping = " + ping.ToString("0.0") + " ms";
    }
}

