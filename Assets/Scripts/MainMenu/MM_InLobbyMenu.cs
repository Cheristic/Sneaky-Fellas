using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;

public class MM_InLobbyMenu : MM_Menu
{
    protected override void OnEnter()
    {
        gameObject.SetActive(true);
        if (!NetworkManager.Singleton.IsHost)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        } else
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public async void Action_LeaveLobby()
    {
        await Task.Run(() => SessionInterface.Instance.LeaveLobby());
        manager.ChangeMenu(0);
    }

    protected override void OnExit()
    {
        gameObject.SetActive(false);
    }
}
