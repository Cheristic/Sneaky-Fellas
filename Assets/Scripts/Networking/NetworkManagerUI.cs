using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Services.Lobbies;
using UnityEngine.Events;


public class NetworkManagerUI : NetworkBehaviour
{
    //[SerializeField] private TestRelay testRelay;
    [SerializeField] private TextMeshProUGUI playerCountText;

    public static NetworkManagerUI Instance { get; private set; }

    public NetworkVariable<int> playerNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    [SerializeField] private Button respawnButton;

    [SerializeField] TMP_InputField lobbyCodeInputField;
    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] Matchmaking matchmaking;

    public UnityEvent<GameObject> OnHitStartGameButton;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        playerCountText.SetText("Players: " + playerNum.Value.ToString() + "/4");

        if (!IsHost) return;
        playerNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    public void EnableRespawn()
    {
        respawnButton.gameObject.SetActive(true);
    }

    public void OnLobbyCodeTextChange()
    {
         matchmaking.PlayPrivate(lobbyCodeInputField.text);

    }

    public void OnPlayerNameTextChange()
    {
        matchmaking.UpdatePlayerName(playerNameInputField.text);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnHitStartGameButtonServerRpc()
    {
        Debug.Log("hey");
        OnHitStartGameButtonClientRpc();
    }
    [ClientRpc]
    private void OnHitStartGameButtonClientRpc()
    {
        Debug.Log("hey");
        OnHitStartGameButton.Invoke(gameObject);
    }

}
