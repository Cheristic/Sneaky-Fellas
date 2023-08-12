using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Services.Lobbies;


public class NetworkManagerUI : NetworkBehaviour
{
    //[SerializeField] private TestRelay testRelay;
    [SerializeField] private TextMeshProUGUI playerCountText;

    public NetworkVariable<int> playerNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    [SerializeField] private Button respawnButton;

    [SerializeField] TMP_InputField lobbyCodeInputField;
    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] Matchmaking matchmaking;


    private void Update()
    {
        playerCountText.SetText("Players: " + playerNum.Value.ToString() + "/4");

        if (!IsHost) return;
        playerNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    public void EnableRespawn()
    {
        respawnButton.interactable = true;
    }

    public void OnLobbyCodeTextChange()
    {
         matchmaking.PlayPrivate(lobbyCodeInputField.text);

    }

    public void OnPlayerNameTextChange()
    {
        matchmaking.UpdatePlayerName(playerNameInputField.text);
    }

}
