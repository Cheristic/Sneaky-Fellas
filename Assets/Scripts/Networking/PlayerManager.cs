using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Dynamic;
using Unity.Networking;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private GameObject playerClassPrefab;

    public UnityEvent OnGameStarted;

    public NetworkList<NetworkObjectReference> networkPlayersSpawned = new NetworkList<NetworkObjectReference>();

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
        PreloadDynamicNetworkPrefabs();

    }


    [ServerRpc]
    public void SpawnPlayersServerRpc()
    {

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject newPlayer = Instantiate(playerClassPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Spawning player for " + clientId);

            newPlayer.SetActive(true);

            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            networkPlayersSpawned.Add(newPlayer);


        }
        

        Debug.Log("Total players ="+NetworkManager.Singleton.ConnectedClientsIds.Count);

    }



    [ServerRpc(RequireOwnership = false)]
    public void RespawnPlayersServerRpc()
    {
        for(int i = networkPlayersSpawned.Count-1; i >= 0; i--)
        {
            GameObject alivePlayer = networkPlayersSpawned[i];
            if (alivePlayer != null)
            {
                networkPlayersSpawned.Remove(alivePlayer);
                alivePlayer.GetComponent<NetworkObject>().Despawn();
                Destroy(alivePlayer);
            }
        }
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject newPlayer = Instantiate(playerClassPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Spawning player for " + clientId);

            newPlayer.SetActive(true);

            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            networkPlayersSpawned.Add(newPlayer);

        }
        Debug.Log("Total players =" + NetworkManager.Singleton.ConnectedClientsIds.Count);

    }

    private void PreloadDynamicNetworkPrefabs()
    {
        NetworkManager.Singleton.AddNetworkPrefab(playerClassPrefab);
        NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = true;
    }

}
