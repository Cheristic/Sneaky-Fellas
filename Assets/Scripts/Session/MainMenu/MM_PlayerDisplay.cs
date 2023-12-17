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
        // THIS LINE IS BROKEN BECASUE THIS WHOLE UI AND MENU NAVIGATION NEEDS TO BE MODULARIZED WITH THE SESSION INTERFACE
        //SessionInterface.Instance.currentSession.playerSessionDatabase.onPlayerDatabaseChange += UpdatePlayerDisplay;
    }

    // This function communicates with the Session Database and adds the clientId parameter
    private void UpdatePlayerName(string input) 
    {
        if (SessionInterface.Instance.currentSession == null) return;
        SessionInterface.Instance.currentSession.playerSessionDatabase.UpdatePlayerNameServerRpc(NetworkManager.Singleton.LocalClientId, input);
    }

    public void UpdatePlayerDisplay()
    {
        if (SessionInterface.Instance.currentSession == null) return;
        for (int i = 0; i < m_playerCards.Length; i++)
        {
            if (SessionInterface.Instance.currentSession.playerSessionDatabase.GetPlayerCount() > i)
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
