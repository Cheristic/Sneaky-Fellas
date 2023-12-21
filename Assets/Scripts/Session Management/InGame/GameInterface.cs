using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Top level interface for a Game. Responsible for creating GameData instances, starting rounds, and returning back to Main Menu
/// </summary>
public class GameInterface : NetworkBehaviour
{
    public static GameInterface Instance { get; private set; }

    public GameAssembler gameAssembler; // Owned by server

    internal GameData gameData;
    internal RoundData roundData;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (IsServer) // ONLY SERVER HAS ACCESS TO GAME ASSEMBLER, CLIENTS OWN LOCAL INSTANCE OF DATA BUT CAN ONLY MAKE CHANGES BY PINGING SERVER
        {
            gameAssembler = new();
            gameAssembler.NewRound();
        }

        GameAssembler.TriggerStartRound += StartRound;

    }

    /// <summary>
    /// Called once Assemblers are finished. Starts a round.
    /// </summary>
    public void StartRound()
    {

    }
}
