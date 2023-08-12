using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int currentHealth, maxHealth;

    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;

    [SerializeField] private bool isDead = false;

    private GameObject blackFilterObject;

    public GameObject playerParent;

    public void InitializeHealth(int healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        isDead = false;
        blackFilterObject = GameObject.FindWithTag("Black Filter");
    }

    public void GetHit(int dmg, GameObject sender)
    {
        if (isDead) return;

        if (sender.layer == gameObject.layer) return;

        currentHealth -= dmg;
        
        if (currentHealth > 0)
        {
            OnHitWithReference?.Invoke(sender);
        } else
        {
            OnDeathWithReference?.Invoke(sender);
            isDead = true;
            DespawnPlayerServerRpc();
            playerParent.SetActive(false);
            blackFilterObject.SetActive(false);
            Destroy(playerParent);
            currentHealth = maxHealth;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnPlayerServerRpc()
    {
        playerParent.GetComponent<NetworkObject>().Despawn();
    }
}
