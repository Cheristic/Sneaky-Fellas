using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }

        PreloadItemDynamicNetworkPrefabs();
    }

    public GameObject itemSpawnPrefab;

    public GameObject[] itemSpawnPoints;

    public GameObject gunPrefab;

    public GameObject bulletPrefab;

    [ServerRpc]
    public void SpawnItemsServerRpc()
    {
        foreach (GameObject itemSpawnPoint in itemSpawnPoints)
        {
            if (Random.Range(0, 4) == 0)
            {
                GameObject newGun = Instantiate(gunPrefab, itemSpawnPoint.transform.position, Quaternion.identity);
                newGun.GetComponent<NetworkObject>().Spawn();
            } else if (Random.Range(0, 2) == 0)
            {
                GameObject newItem = Instantiate(itemSpawnPrefab, itemSpawnPoint.transform.position, Quaternion.identity);
                newItem.GetComponent<NetworkObject>().Spawn();
            }
                     
        }

    }

    [ServerRpc]
    public void DeleteSpawnedItemsServerRpc()
    {
        ItemClass[] spawneditems = FindObjectsOfType<ItemClass>();
        foreach (ItemClass itemClass in spawneditems)
        {
            itemClass.GetComponent<NetworkObject>().Despawn();
            Destroy(itemClass);
        }
    }

    private void PreloadItemDynamicNetworkPrefabs()
    {
        NetworkManager.Singleton.AddNetworkPrefab(itemSpawnPrefab);
        NetworkManager.Singleton.AddNetworkPrefab(gunPrefab);
        NetworkManager.Singleton.AddNetworkPrefab(bulletPrefab);
    }
}
