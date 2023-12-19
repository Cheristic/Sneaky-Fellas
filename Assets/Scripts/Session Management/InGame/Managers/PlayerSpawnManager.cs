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
    [SerializeField] private GameObject playerClassPrefab;

    public List<NetworkObject> playersSpawned = new();

    private static System.Random rng = new System.Random();
    List<Transform> shuffledSpawnPoints;


    void Awake()
    {
        PreloadPlayerDynamicNetworkPrefabs();
    }


    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayersServerRpc()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        playersSpawned.Clear();
        ShufflePlayerSpawnPoints();

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

    [ServerRpc(RequireOwnership = false)]
    public void RespawnPlayersServerRpc()
    {
        // Clear any players already spawned
        for (int i = playersSpawned.Count - 1; i >= 0; i--)
        {
            if (playersSpawned[i] is not null)
            {
                GameObject alivePlayer = playersSpawned[i].gameObject;
                alivePlayer.GetComponent<NetworkObject>().Despawn();
                Destroy(alivePlayer);
            }
        }

        SpawnPlayers();
    }


    private void ShufflePlayerSpawnPoints()
    {
        shuffledSpawnPoints = new List<Transform>();

        Transform spawnPointParent = GameObject.FindGameObjectWithTag("Spawn Point Parent").transform;
        foreach (Transform child in spawnPointParent)
        {
            shuffledSpawnPoints.Add(child);
        }
        shuffledSpawnPoints = shuffledSpawnPoints.OrderBy(x => rng.Next()).ToList();
    }

    private void PreloadPlayerDynamicNetworkPrefabs()
    {
        NetworkManager.Singleton.AddNetworkPrefab(playerClassPrefab);
        NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = false;
    }

    public void AddPlayerObjectToList(Component sender, object data)
    {
        playersSpawned.Add((NetworkObject)data);
    }

    public void RemovePlayerObjectFromList(Component sender, object data)
    {
        playersSpawned[playersSpawned.IndexOf((NetworkObject)data)] = null;
    }


}
