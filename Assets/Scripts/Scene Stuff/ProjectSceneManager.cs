using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

public class ProjectSceneManager : NetworkBehaviour
{
    private Dictionary<MapChoice, string> mapChoices = new Dictionary<MapChoice, string>
    {
        {MapChoice.Default, "Default Map" }
    };
    private string currMapChoice;

    public override void OnNetworkSpawn()
    { 
        DontDestroyOnLoad(this);
        MatchmakingCommands.startGameScene += SwitchToLoadScene;
        NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled = true;
    }

    private void SwitchToLoadScene(MapChoice map)
    {
        currMapChoice = mapChoices[map];
        var status = NetworkManager.Singleton.SceneManager.LoadScene("Loading", LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.Log($"Failed to load {currMapChoice}");
            return;
        }
        NetworkManager.Singleton.SceneManager.OnSceneEvent += BeginLoad;
    }

    // Once in Load screen, go to selected map
    private void BeginLoad(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType != SceneEventType.LoadEventCompleted) return;
        NetworkManager.Singleton.SceneManager.OnSceneEvent -= BeginLoad; // Reset scene events 
        LoadMap(); 
    }

    private void LoadMap()
    {
        var status = NetworkManager.Singleton.SceneManager.LoadScene(currMapChoice, LoadSceneMode.Additive);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.Log($"Failed to load {currMapChoice}");
            return;
        }

        NetworkManager.Singleton.SceneManager.OnSceneEvent += FinishLoadMap;

        GameAssembler.TriggerStartRound += EndLoad;
    }

    private void FinishLoadMap(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType != SceneEventType.LoadEventCompleted) return;
        NetworkManager.Singleton.SceneManager.OnSceneEvent -= FinishLoadMap;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("currMapChoice"));
    }

    // Called by game scene, close Loading scene once game is ready
    private void EndLoad()
    {
        GameAssembler.TriggerStartRound -= EndLoad;
        currMapChoice = null;
        var status = NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName("Loading"));
    }
}
