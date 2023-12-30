using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Linq;
public class PlayerSpawner : NetworkBehaviour
{
    private GameObject playerClassPrefab;


    private static System.Random rng = new System.Random();
    private List<GameObject> playerSpawnPoints;

    private void Awake()
    {
        playerClassPrefab = (GameObject)Resources.Load("InGame/Player/Player");
        playerSpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player Spawn Point"));        
    }

    public void NewRound(ref List<GameObject> players)
    {
        // SPAWN PLAYERS
        ShufflePlayerSpawnPoints();
        ClearPlayerObjects();

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject newPlayer = Instantiate(playerClassPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Spawning player for " + clientId);

            var pos = playerSpawnPoints[(int)clientId].transform.position;
            newPlayer.GetComponent<PlayerInterface>().playerObject.transform.position = new Vector3(pos.x, pos.y, pos.z);

            newPlayer.SetActive(true);

            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            players.Add(newPlayer);
            //spawnPlayer?.Invoke(newPlayer); // Pass new player to Round Data

        }
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
