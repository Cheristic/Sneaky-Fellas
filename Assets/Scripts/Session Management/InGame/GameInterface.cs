using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

/// <summary>
/// Top level interface for a Game. Responsible for creating GameData instances, starting rounds, and returning back to Main Menu
/// </summary>
public class GameInterface : NetworkBehaviour
{
    public static GameInterface Instance { get; private set; }

    // Assemblers used to create new games + rounds
    public GameAssembler gameAssembler; // Owned by server
    public RoundAssembler roundAssembler;

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

    public void PrepareGame() // Called by ProjectSceneManager once scene is loaded for all clients
    {
        // ONLY SERVER HAS ACCESS TO ASSEMBLERS, CLIENTS OWN LOCAL INSTANCE OF DATA BUT CAN ONLY MAKE CHANGES BY PINGING SERVER
        gameAssembler = new();
    }

    /// <summary>
    /// Called once Assemblers are finished. Starts a round.
    /// </summary>
    public void StartRound()
    {

    }
}
