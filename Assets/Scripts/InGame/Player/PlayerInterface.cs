using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Top level interface for all player objects.
/// </summary>
public class PlayerInterface : NetworkBehaviour
{
    public static PlayerInterface Main { get; private set; }

    [HideInInspector] public ulong clientId; // The ID of the client this player belongs to
    [HideInInspector] public bool IsMainPlayer;

    public GameObject playerObject;
    [HideInInspector] public GameObject playerAddOn;

    [HideInInspector] public PlayerHealth playerHealth;
    public PlayerSpriteManager playerSprite;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (Main != null && Main != this)
            {
                Destroy(this);
            }
            else
            {
                Main = this;
            }
        }
    }
}
