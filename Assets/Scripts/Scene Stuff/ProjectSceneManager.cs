using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;


public class ProjectSceneManager : NetworkBehaviour
{
    public static ProjectSceneManager Instance { get; private set; }

    private enum LoadPhase
    {
        InMainMenu,
        ToLoadScene,
        ToGameScene,
        InGameScene
    }

    private LoadPhase currentPhase = LoadPhase.InMainMenu;

    private Dictionary<MapChoice, string> mapChoices = new Dictionary<MapChoice, string>
    {
        {MapChoice.Default, "Default Map" }
    };
    private string currMapChoice;

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

        DontDestroyOnLoad(this);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        NetworkManager.SceneManager.ActiveSceneSynchronizationEnabled = true;
        MatchmakingCommands.startGameScene += BeginLoadToGame; // Called once the server triggers the game to start
        NetworkManager.SceneManager.OnSceneEvent += HandleLoadPhases;
    }

    private void BeginLoadToGame(MapChoice map)
    {
        currMapChoice = mapChoices[map];
        currentPhase = LoadPhase.ToLoadScene;
        SceneEventProgressStatus status = NetworkManager.SceneManager.LoadScene("Loading", LoadSceneMode.Single); // Begin load for all clients
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.Log($"Failed to load {currMapChoice}");
            return;
        }
    }

    private void HandleLoadPhases(SceneEvent sceneEvent)
    {
        bool IsServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? true : false;
        if (!IsServer) return; // Only Server is allowed to manage the Scene Event Phase

        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            switch (currentPhase)
            {
                // If phase is somehow InMainMenu it'll not do anything
                case LoadPhase.ToLoadScene:  // Loading scene has loaded for all clients, now go to Game
                    var status = NetworkManager.Singleton.SceneManager.LoadScene(currMapChoice, LoadSceneMode.Additive);
                    if (status != SceneEventProgressStatus.Started)
                    {
                        Debug.Log($"Failed to load {currMapChoice}");
                        return;
                    }
                    currentPhase += 1; // Transition to next phase
                    break;
                case LoadPhase.ToGameScene: // Game is loaded for all clients, now prepare the game
                    currentPhase += 1;
                    RoundAssembler.TriggerStartFirstRound += GameStarting; // Once game is prepped
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(currMapChoice));
                    GameInterface.Instance.PrepareGame(); // Only called by server
                    break;
            }

        }
    }

    // Called once First Round has finished assembling and is about to start
    private void GameStarting()
    {
        RoundAssembler.TriggerStartFirstRound -= GameStarting;
        var status = NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneByName("Loading"));
    }
}
