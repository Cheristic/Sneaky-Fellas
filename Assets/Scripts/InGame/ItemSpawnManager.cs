using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemSpawnManager : MonoBehaviour
{
    public GameObject itemSpawnPrefab;
    public GameObject[] itemSpawnPoints;

    [ServerRpc]
    public void SpawnItemsServerRpc()
    {
        foreach (GameObject itemSpawnPoint in itemSpawnPoints)
        {
            GameObject newItem = Instantiate(itemSpawnPrefab, itemSpawnPoint.transform.position, Quaternion.identity);
            newItem.GetComponent<NetworkObject>().Spawn();
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
}
