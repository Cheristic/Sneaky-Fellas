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
        i.playerAddOn.GetComponent<NetworkObject>().Spawn(true);
        i.playerAddOn.transform.parent = transform;
        i.playerAddOn.GetComponent<NetworkObject>().TrySetParent(transform);
        i.clientId = OwnerClientId;
    }

    public override void OnDestroy()
    {
        PlayerInterface i = GetComponent<PlayerInterface>();
        i.playerAddOn.GetComponent<NetworkObject>().Despawn(true);
        i.playerAddOn.GetComponent<NetworkObject>().TryRemoveParent();
        base.OnDestroy();
    }
}
