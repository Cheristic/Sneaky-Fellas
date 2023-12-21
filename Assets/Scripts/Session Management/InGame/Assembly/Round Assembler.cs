using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Soon as a Round starts, this prepares the game and tells the interface to begin the round.
/// Once variations come into play, this is what will be responsible for implementing them.
/// Basically serves as an abstract layer between Game Assembler and RoundData.
/// </summary>
public class RoundAssembler
{
    private PlayerSpawner playerSpawner;
    private ItemSpawner itemSpawner;
    public RoundAssembler()
    {
        playerSpawner = GameInterface.Instance.gameObject.AddComponent<PlayerSpawner>();
        //itemSpawner = new();
    }

    public void BuildRound(int seed)
    {
        GameInterface.Instance.roundData.NewRound(); // This is probably bad practice to reference the Interface
        // Server will update clients local instance of roundData whenever there's a change and this is the easiest method.

        GameInterface.Instance.roundData.playersSpawned = playerSpawner.NewRound();
    }

}
