using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Services.Lobbies.Models;
using System;

public class MM_UIManager : NetworkBehaviour
{
    Animator animator;

    public enum MenuScreens
    {
        Home,
        InLobby,
    }

    public List<Canvas> canvases = new List<Canvas>(); 
    /*  Home = 0
        InLobby = 1
        InLobbyHost = 2
        PlayerDisplay = 3
    */

    public MenuScreens screen;
    private void Start()
    {
        animator = GetComponent<Animator>();
        screen = MenuScreens.Home;
    }

    public void GoTo_InLobby()
    {
        screen = MenuScreens.InLobby;
        animator.SetTrigger("InLobby");
        foreach (Canvas canvas in canvases)
        {
            if (canvas == canvases[1]) canvas.gameObject.SetActive(true);
            else if (canvas == canvases[2] && IsHost) canvas.gameObject.SetActive(true);
            else if (canvas == canvases[3]) canvas.gameObject.SetActive(true);
            else canvas.gameObject.SetActive(false);
        }
    }

    public void GoTo_Home()
    {
        screen = MenuScreens.Home;
        animator.SetTrigger("Home");

        foreach (Canvas canvas in canvases)
        {
            if (canvas == canvases[0]) canvas.gameObject.SetActive(false);
            else canvas.gameObject.SetActive(true);
        }
    }

    // Events
    public void HitJoinButton(TextMeshProUGUI lobbyCodeInputFieldText)
    {
        string c = lobbyCodeInputFieldText.text.Substring(0, lobbyCodeInputFieldText.text.Length-1); // slices off last character
        SessionInterface.Instance.JoinPrivate(c);
    }

    // THIS FUNCTION IS CURRENTLY NOT CONNECTED TO ANYTHING
    // Rework with modularized UI but in the meantime can just output to console for now
    public void onJoinLobby(Component sender, object data) // Displays lobby code
    {
        Lobby l = (Lobby)data;
        canvases[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = l.LobbyCode;
        GoTo_InLobby();
    }
}
