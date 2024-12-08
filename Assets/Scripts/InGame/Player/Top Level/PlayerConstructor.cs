using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerConstructor : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject MainPlayerPrefab;
    [SerializeField] private GameObject NetworkPlayerPrefab;

    PlayerInterface i;

    private void Start()
    {
        i = GetComponent<PlayerInterface>();
        i.playerObject.transform.position = i.playerSpawnLocation;
        i.IsMainPlayer = IsOwner;
        
        i.playerId = OwnerClientId;

        SyncGameData.TriggerNewRoundReady.AddListener(OnTriggerNewRoundReady);

        if (IsOwner)
        {
            i.playerAddOn = Instantiate(MainPlayerPrefab, transform); // Give player the MainPlayer Module
        }
        else
        {
            i.playerObject.layer = LayerMask.NameToLayer("BehindMask");
            i.playerSprite.gameObject.layer = LayerMask.NameToLayer("BehindMask");
            i.playerAddOn = Instantiate(NetworkPlayerPrefab, transform); // Give player the NetworkPlayer Module
        }
    }

    private void OnTriggerNewRoundReady()
    {
        i.playerObject.transform.position = i.playerSpawnLocation;
        i.playerObject.layer = 11; // Player
        i.playerSprite.gameObject.layer = 11;

        i.playerObject.GetComponent<PolygonCollider2D>().enabled = true;
    }

    public override void OnDestroy()
    {
        PlayerInterface i = GetComponent<PlayerInterface>();
        base.OnDestroy();
    }
}
