using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerConstructor : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject MainPlayerPrefab;
    [SerializeField] private GameObject OtherPlayerPrefab;

    public override void OnNetworkSpawn()
    {
        PlayerInterface i = GetComponent<PlayerInterface>();
        i.IsMainPlayer = IsOwner;
        if (IsOwner)
        {
            i.playerAddOn = Instantiate(MainPlayerPrefab); // Give player the MainPlayer Module
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("BehindMask");
            i.playerAddOn = Instantiate(OtherPlayerPrefab); // Give player the OtherPlayer Module
        }
        i.playerAddOn.transform.parent = transform;
        i.clientId = OwnerClientId;
    }
}
