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

    [SerializeField] private AssetReferenceGameObject playerPrefabAsset;

    //public NetworkObject playerPrefabNet;

    private GameObject playerPrefab;
    public GameObject playerPrefabNO;

    async void Start()
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
        await PreloadDynamicPlayerPrefab();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayersServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //if (!playerPrefabNO) return;
        Debug.Log("Spawning player");
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        //ulong clientId = NetworkManager.Singleton.LocalClientId;
        SpawnPlayersClientRpc(clientId);
        Debug.Log("playerPrefab = " + playerPrefabNO.name);
        GameObject newPlayer = Instantiate(playerPrefabNO, Vector3.zero, Quaternion.identity);

        newPlayer.SetActive(true);

        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        
        
        //ulong objectId = playerInstance.GetComponent<NetworkObject>().NetworkObjectId;
        //PlayerData.Add(objectId, playerInstance);
        //InstantiatePlayersClientRpc(objectId);
    }

    [ClientRpc]
    public void SpawnPlayersClientRpc(ulong clientId)
    {
        Debug.Log("Logging in player: " + clientId);
    }

    async Task PreloadDynamicPlayerPrefab()
    {
        /*Debug.Log($"Started to load addressable with GUID: {playerPrefabAsset.AssetGUID}");
        var op = Addressables.LoadAssetAsync<GameObject>(playerPrefabAsset);
        playerPrefab= await op.Task;
        Addressables.Release(op);
        
        //it's important to actually add the player prefab to the list of network prefabs - it doesn't happen
        //automatically
        NetworkManager.Singleton.AddNetworkPrefab(playerPrefabNO);
        Debug.Log($"Loaded prefab has been assigned to NetworkManager's PlayerPrefab");

        // at this point we can easily change the PlayerPrefab
        NetworkManager.Singleton.NetworkConfig.PlayerPrefab = playerPrefabNO;
        */
        // Forcing all game instances to load a set of network prefabs and having each game instance inject network
        // prefabs to NetworkManager's NetworkPrefabs list pre-connection time guarantees that all players will have
        // matching NetworkConfigs. This is why NetworkManager.ForceSamePrefabs is set to true. We let Netcode for
        // GameObjects validate the matching NetworkConfigs between clients and the server. If this is set to false
        // on the server, clients may join with a mismatching NetworkPrefabs list from the server. 
        NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = true;
    }

}
