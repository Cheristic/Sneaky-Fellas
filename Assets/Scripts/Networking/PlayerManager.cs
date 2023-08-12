using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Dynamic;
using Unity.Networking;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }

    //Dictionary<ulong, GameObject> playerdict;

    //public NetworkVariable<Dictionary> PlayerData = new NetworkVariable<Dictionary>()<ulong, GameObject> { get => playerdict; set => playerdict = value; }

    //public NetworkObject playerPrefabNet;

    [SerializeField] private GameObject playerClassPrefab;

    void Start()
    {
        Debug.Log("Starting Player Manager");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
        PreloadDynamicPlayerPrefab();
    }


    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayersServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //if (!playerClassPrefab) return;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject newPlayer = Instantiate(playerClassPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Spawning player for " + clientId);

            newPlayer.SetActive(true);

            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        }
        Debug.Log("Total players ="+NetworkManager.Singleton.ConnectedClientsIds.Count);
       // ulong clientId = serverRpcParams.Receive.SenderClientId;
        //ulong clientId = NetworkManager.Singleton.LocalClientId;
        
        
        //ulong objectId = playerInstance.GetComponent<NetworkObject>().NetworkObjectId;
        //PlayerData.Add(objectId, playerInstance);
        //InstantiatePlayersClientRpc(objectId);
    }

    private void PreloadDynamicPlayerPrefab()
    {
        /*Debug.Log($"Started to load addressable with GUID: {playerPrefabAsset.AssetGUID}");
        var op = Addressables.LoadAssetAsync<GameObject>(playerPrefabAsset);
        playerPrefab= await op.Task;
        Addressables.Release(op);
        
        //it's important to actually add the player prefab to the list of network prefabs - it doesn't happen
        //automatically
        NetworkManager.Singleton.AddNetworkPrefab(playerClassPrefab);
        Debug.Log($"Loaded prefab has been assigned to NetworkManager's PlayerPrefab");

        // at this point we can easily change the PlayerPrefab
        NetworkManager.Singleton.NetworkConfig.PlayerPrefab = playerClassPrefab;
        */
        // Forcing all game instances to load a set of network prefabs and having each game instance inject network
        // prefabs to NetworkManager's NetworkPrefabs list pre-connection time guarantees that all players will have
        // matching NetworkConfigs. This is why NetworkManager.ForceSamePrefabs is set to true. We let Netcode for
        // GameObjects validate the matching NetworkConfigs between clients and the server. If this is set to false
        // on the server, clients may join with a mismatching NetworkPrefabs list from the server.
        NetworkManager.Singleton.AddNetworkPrefab(playerClassPrefab);
        NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = true;
    }

}
