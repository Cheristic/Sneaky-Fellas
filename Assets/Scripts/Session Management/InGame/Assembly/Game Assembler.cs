using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

/// <summary>
/// Created when the Game initially starts. It does all the game prep + round prep for each round.
/// </summary>
public class GameAssembler
{
    private RoundAssembler roundAssembler;

    /// <summary>
    /// Causes Loading scene to unload and begins any timer (EVENTUALLY) for round to start.
    /// </summary>
    public static event Action TriggerStartRound;

    // ############# GAME SCENE STARTS #############
    public GameAssembler()
    {
        GameData gameData = new(); // Create the game data
        DistributeGameData.DistributeGameData_ClientRpc(GameInterface.Instance.gameData);

        GameInterface.Instance.roundData = new(); // Create server's Round Data
        roundAssembler = new(); // Build round
    }

    // ############# ROUND STARTS #############
    public void NewRound()
    {
        roundAssembler.BuildRound(0);
        DistributeGameData.DistributeRoundData_ClientRpc(GameInterface.Instance.roundData); // Distribute server's Round Data
        TriggerStartRound_ClientRpc();
    }

    [ClientRpc]
    private void TriggerStartRound_ClientRpc()
    {
        TriggerStartRound?.Invoke();
    }

}

/// <summary>
/// Helper methods used to distribute the server's instance of GameData + RoundData.
/// </summary>
public static class DistributeGameData
{
    [ClientRpc]
    public static void DistributeGameData_ClientRpc(GameData gameData)
    {
        GameInterface.Instance.gameData = gameData;
    }

    [ClientRpc]
    public static void DistributeRoundData_ClientRpc(RoundData roundData)
    {
        roundData.playersSpawned = JsonCommands.GameObjectListFromJson(roundData.jsonPlayersSpawned); // Deserialize
        GameInterface.Instance.roundData = roundData;
    }
}
