using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

/// <summary>
/// Created when the Game initially starts. It does all the game prep + round prep + distribution for each round
/// Abstraction layer between Game Interface and Round Assembler/Data
/// </summary>
public class GameAssembler
{
    private GameData gameData; // Server instance of Game Data


    // ############# GAME SCENE STARTS #############
    public GameAssembler()
    {
        gameData = new(); // Create the game data
        DistributeGameData_ClientRpc(gameData); // Distribute server's game data

    }

    public void NewRound()
    {
        //roundAssembler.NewRound();
    }

    // ############# RPC + HELPER METHODS #############
    [ClientRpc]
    private static void DistributeGameData_ClientRpc(GameData gameData)
    {
        GameInterface.Instance.gameData = gameData;
    }


}
