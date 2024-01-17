using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

/// <summary>
/// Top level interface for a Game. Responsible for creating GameData instances, starting rounds, and returning back to Main Menu
/// </summary>
public class InGameDataInterface : NetworkBehaviour
{
    public static InGameDataInterface Instance { get; private set; }

    internal GameData gameData;
    internal RoundData roundData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
