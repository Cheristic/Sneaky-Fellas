using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Linq;
using Unity.Multiplayer.Samples.Utilities;
public class PlayerSpawner : NetworkBehaviour
{
    private GameObject playerClassPrefab;


    private static System.Random rng = new System.Random();
    private List<GameObject> playerSpawnPoints;

    public void Init()
    {
        playerClassPrefab = (GameObject)Resources.Load("InGame/Player/Player");
        playerSpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player Spawn Point"));        
    }

    public void FirstRound(ref RoundData roundData)
    {
        // SPAWN PLAYERS
        ShufflePlayerSpawnPoints();

        foreach (PlayerSessionData player in SessionInterface.Instance.currentSession.players)
        {
            GameObject newPlayer = Instantiate(playerClassPrefab, Vector3.zero, Quaternion.identity);

            var pos = playerSpawnPoints[(int)player.LocalClientId].transform.position;
            newPlayer.GetComponent<PlayerInterface>().playerSpawnLocation = new Vector3(pos.x, pos.y, pos.z); // Set initial spawn zone

            newPlayer.SetActive(true);

            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(player.LocalClientId, true); // Players will begin constructing themselves

            roundData.players.Add(newPlayer.GetComponent<PlayerInterface>()); // Pass new player to Round Data
        }

        for (int i = 0; i < roundData.players.Count; i++)
        {
            roundData.playerNetworkObjects[i] = roundData.players[i].GetComponent<NetworkObject>();
        }
    }

    public void NewRound(ref RoundData roundData)
    {
        ShufflePlayerSpawnPoints();
        // Reset Player info and place in different spawn location
        foreach (PlayerSessionData player in SessionInterface.Instance.currentSession.players)
        {
            var pos = playerSpawnPoints[(int)player.LocalClientId].transform.position;
            PlayerInterface i = roundData.players[(int)player.LocalClientId].GetComponent<PlayerInterface>();
            i.playerSpawnLocation = new Vector3(pos.x, pos.y, pos.z);
        }
    }

    private void ShufflePlayerSpawnPoints()
    {
        playerSpawnPoints = playerSpawnPoints.OrderBy(x => rng.Next()).ToList();
    }
}
