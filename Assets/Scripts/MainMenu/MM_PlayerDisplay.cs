using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using TMPro;
using UnityEngine.Events;

public class MM_PlayerDisplay : NetworkBehaviour
{
    //public static MM_PlayerDisplay Instance { get; private set; }

    [SerializeField] private MM_PlayerCard[] m_playerCards;

    [SerializeField] private GameObject playerNameInputField;

    private UnityAction<string> m_OnEndPlayerNameEdit;

    private void Start()
    {
        /*if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }*/
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            m_OnEndPlayerNameEdit += UpdatePlayerName;
            playerNameInputField.GetComponent<TMP_InputField>().onEndEdit.AddListener(m_OnEndPlayerNameEdit);

            playerNameInputField.SetActive(true);
        }
    }

    private void UpdatePlayerName(string input) 
    {
        if (Matchmaking.Instance.joinedLobby != null)
        {
            PlayerGameDatabase.Instance.UpdatePlayerNameServerRpc(NetworkManager.Singleton.LocalClientId, input);
            UpdatePlayerDisplay();
        }
    }

    public void UpdatePlayerDisplay()
    {
        for (int i = 0; i < m_playerCards.Length; i++)
        {
            if (PlayerGameDatabase.Instance.GetPlayerCount() > i)
            {
                
                m_playerCards[i].UpdatePlayerCard(i);
            } 
            else
            {
                m_playerCards[i].DisablePlayerCard();
            }
        }
    }
}
