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

    [SerializeField] private MM_PlayerCard[] m_playerCards;

    [SerializeField] private GameObject playerNameInputField;

    private UnityAction<string> m_OnEndPlayerNameEdit;

    public void Start()
    {
        m_OnEndPlayerNameEdit += UpdatePlayerName;
        playerNameInputField.GetComponent<TMP_InputField>().onEndEdit.AddListener(m_OnEndPlayerNameEdit);
    }

    // This function communicates with the Session Database and adds the clientId parameter
    private void UpdatePlayerName(string input) 
    {
        PlayerSessionDatabase.Instance.UpdatePlayerNameServerRpc(NetworkManager.Singleton.LocalClientId, input);
    }

    public void UpdatePlayerDisplay()
    {
        for (int i = 0; i < m_playerCards.Length; i++)
        {
            if (PlayerSessionDatabase.Instance.GetPlayerCount() > i)
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
