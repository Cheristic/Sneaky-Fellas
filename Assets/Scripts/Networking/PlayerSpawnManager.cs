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
using UnityEngine.SceneManagement;
using System.Threading;
using System.Linq;
public class PlayerSpawnManager : NetworkBehaviour
{
    public static PlayerSpawnManager Instance { get; private set; }

    [SerializeField] private GameObject playerClassPrefab;
    [SerializeField] private GameObject playerDisplay;

    public UnityEvent OnGameStarted;

    public List<NetworkObject> networkPlayersSpawned = new();

    private static System.Random rng = new System.Random();
    List<Transform> shuffledSpawnPoints;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
        PreloadPlayerDynamicNetworkPrefabs();

    }


    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayersServerRpc()
    {
        ShuffleSpawnPoints();

        ItemSpawnManager.Instance.SpawnItemsServerRpc();

        SpawnPlayers();

    }



    [ServerRpc(RequireOwnership = false)]
    public void RespawnPlayersServerRpc()
    {
        ShuffleSpawnPoints();
        ItemSpawnManager.Instance.DeleteSpawnedItemsServerRpc();
        ItemSpawnManager.Instance.SpawnItemsServerRpc();

        for (int i = networkPlayersSpawned.Count-1; i >= 0; i--)
        {
            if (networkPlayersSpawned[i] is not null)
            {
                GameObject alivePlayer = networkPlayersSpawned[i].gameObject;
                alivePlayer.GetComponent<NetworkObject>().Despawn();
                Destroy(alivePlayer);
            }
        }
        ResetNetworkPlayersSpawnedListClientRpc();

        SpawnPlayers();
    }

    [ClientRpc]
    private void ResetNetworkPlayersSpawnedListClientRpc()
    {
        networkPlayersSpawned = new();
    }


    private void SpawnPlayers()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject newPlayer = Instantiate(playerClassPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Spawning player for " + clientId);

            var pos = shuffledSpawnPoints[(int)clientId].position;
            newPlayer.transform.Find("Player").gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z);

            newPlayer.SetActive(true);

            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }


    private void ShuffleSpawnPoints()
    {
        shuffledSpawnPoints = new List<Transform>();

        Transform spawnPointParent = GameObject.FindGameObjectWithTag("Spawn Point Parent").transform;
        foreach (Transform child in spawnPointParent)
        {
            shuffledSpawnPoints.Add(child);
        }
        shuffledSpawnPoints = shuffledSpawnPoints.OrderBy(x => rng.Next()).ToList();
    }

    public int GetIdByPlayerObject(GameObject go)
    {
        try
        {
            return networkPlayersSpawned.IndexOf(go.GetComponentInParent<NetworkObject>());
        } catch
        {
            return -1;
        }
    }


    private void PreloadPlayerDynamicNetworkPrefabs()
    {
        NetworkManager.Singleton.AddNetworkPrefab(playerClassPrefab);
        NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = false;
    }


}
