using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MM_PlayerCard : MonoBehaviour
{
    [SerializeField] private GameObject visuals;
    [SerializeField] private TMP_Text playerNameText;

    public void UpdatePlayerCard(int clientNum)
    {
        PlayerSessionData data = SessionInterface.Instance.currentSession.players[clientNum];
        playerNameText.text = data.PlayerName.ToString();

        visuals.SetActive(true);

    }

    public void DisablePlayerCard()
    {
        visuals.SetActive(false);
    }
}
