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
using static UnityEditor.FilePathAttribute;
using UnityEngine.UIElements;
public class PlayerSpawner : NetworkBehaviour
{
    private GameObject playerClassPrefab;


    private static System.Random rng = new System.Random();
    private List<GameObject> playerSpawnPoints;

    private void Awake()
    {
        playerClassPrefab = (GameObject)Resources.Load("Player/PlayerClass");
        playerSpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player Spawn Point"));        
    }

    public static event Action<GameObject> spawnPlayer;
    public List<GameObject> NewRound()
    {
        // SPAWN PLAYERS
        ShufflePlayerSpawnPoints();
        ClearPlayerObjects();
        List<GameObject> players = new();

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject newPlayer = Instantiate(playerClassPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Spawning player for " + clientId);

            var pos = playerSpawnPoints[(int)clientId].transform.position;
            newPlayer.transform.Find("Player").gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z);

            newPlayer.SetActive(true);

            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            players.Add(newPlayer);
            //spawnPlayer?.Invoke(newPlayer); // Pass new player to Round Data

        }
        return players;
    }

    private void ClearPlayerObjects()
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values)
        {
            if (client.PlayerObject is not null)
            {
                client.PlayerObject.Despawn();
                Destroy(client.PlayerObject.gameObject);
            }
        }
    }

    private void ShufflePlayerSpawnPoints()
    {
        playerSpawnPoints = playerSpawnPoints.OrderBy(x => rng.Next()).ToList();
    }
}
