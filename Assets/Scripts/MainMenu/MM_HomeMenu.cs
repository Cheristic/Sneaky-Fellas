using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MM_HomeMenu : MM_Menu
{
    TMP_InputField lobbyCodeInput;
    private void Start()
    {
        lobbyCodeInput = transform.GetChild(2).GetComponent<TMP_InputField>();
    }
    protected override void OnEnter()
    {
        gameObject.SetActive(true);
    }

    public async void Action_Host()
    {
        bool successful = await SessionInterface.Instance.Host();
        if (successful) manager.ChangeMenu(1);
    }
    public async void Action_Join()
    {
        bool successful = await SessionInterface.Instance.JoinPrivate(lobbyCodeInput.text);
        if (successful) manager.ChangeMenu(1);
    }

    protected override void OnExit()
    {
        gameObject.SetActive(false);
    }
}
