using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Services.Lobbies.Models;
using System;

public class MM_UIManager : MonoBehaviour
{
    public List<MM_Menu> menus;
    private void Start()
    {

        ChangeMenu(0);
    }

    MM_Menu currentMenu;
    public void ChangeMenu(int menuIndex)
    {
        if (currentMenu != null)
        {
            currentMenu.OnMenuExit();
        }
        currentMenu = menus[menuIndex];
        currentMenu.OnMenuEnter(this);
    }
}

public abstract class MM_Menu : MonoBehaviour
{
    protected MM_UIManager manager;
    public void OnMenuEnter(MM_UIManager m)
    {
        manager = m;
        OnEnter();
    }
    protected virtual void OnEnter() { }
    
    public void OnMenuUpdate()
    {
        OnUpdate();
    }
    protected virtual void OnUpdate() { }

    public void OnMenuExit()
    {
        OnExit();
    }
    protected virtual void OnExit() { }
}

