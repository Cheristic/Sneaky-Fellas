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

    [HideInInspector] public ulong playerId; // The static id (1, 2, 3, 4, etc.) allows for rejoining
    [HideInInspector] public bool IsMainPlayer;

    public Vector3 playerSpawnLocation;
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
