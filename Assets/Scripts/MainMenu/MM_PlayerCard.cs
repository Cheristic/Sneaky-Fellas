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
        PlayerData data = PlayerGameDatabase.Instance.GetPlayerDataByIndex(clientNum);
        playerNameText.text = data.PlayerName.ToString();

        visuals.SetActive(true);

    }

    public void DisablePlayerCard()
    {
        visuals.SetActive(false);
    }
}
