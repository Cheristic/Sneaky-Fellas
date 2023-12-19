using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInterface : MonoBehaviour
{
    public GameData gameData;

    private void Start()
    {
        gameData.GameStarting();
    }

    public void RestartRound() => gameData.RestartRound();
}
