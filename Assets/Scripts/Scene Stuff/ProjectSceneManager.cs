using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ProjectSceneManager : NetworkBehaviour
{
    //public static ProjectSceneManager Instance { get; private set; }

    public string MapSceneToLoad;
    [SerializeField] private Scene m_LoadedScene;
    /*void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }*/

    public override void OnNetworkSpawn()
    {
        NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
        MatchmakingCommands.changeToScene += ChangeToScene;
    }
    public bool SceneIsLoaded
    {
        get
        {
            if (m_LoadedScene.IsValid() && m_LoadedScene.isLoaded)
            {
                return true;
            }
            return false;
        }
    }

    public void ChangeToScene(string  mapName)
    {
        // CURRENTLY ONLY ACCESSES THE DEFAULT MAP
        ChangeToMapScene();
    }
    public void ChangeToMapScene()
    {
        var status = NetworkManager.SceneManager.LoadScene(MapSceneToLoad, LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.Log($"Failed to load {MapSceneToLoad}");
        }
    }

    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        var clientOrServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? "server" : "client";
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadComplete:
                {
                    // We want to handle this for only the server-side
                    if (sceneEvent.ClientId == NetworkManager.ServerClientId)
                    {
                        // *** IMPORTANT ***
                        // Keep track of the loaded scene, you need this to unload it
                        m_LoadedScene = sceneEvent.Scene;
                    }
                    Debug.Log($"Loaded the {sceneEvent.SceneName} scene on " +
                        $"{clientOrServer}-({sceneEvent.ClientId}).");
                    break;
                }
            case SceneEventType.UnloadComplete:
                {
                    Debug.Log($"Unloaded the {sceneEvent.SceneName} scene on " +
                        $"{clientOrServer}-({sceneEvent.ClientId}).");
                    break;
                }
            case SceneEventType.LoadEventCompleted:
            case SceneEventType.UnloadEventCompleted:
                {
                    var loadUnload = sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted ? "Load" : "Unload";
                    Debug.Log($"{loadUnload} event completed for the following client " +
                        $"identifiers:({sceneEvent.ClientsThatCompleted})");
                    if (sceneEvent.ClientsThatTimedOut.Count > 0)
                    {
                        Debug.LogWarning($"{loadUnload} event timed out for the following client " +
                            $"identifiers:({sceneEvent.ClientsThatTimedOut})");
                    }
                    break;
                }
        }
    }

    public void UnloadScene()
    {
        if (!IsServer || !IsSpawned || !m_LoadedScene.IsValid() || !m_LoadedScene.isLoaded)
        {
            return;
        }
        var status = NetworkManager.Singleton.SceneManager.UnloadScene(m_LoadedScene);
    }

    public void UnloadSceneSelected(Scene sceneToUnload)
    {
        if (!IsServer || !IsSpawned || !sceneToUnload.IsValid() || !sceneToUnload.isLoaded)
        {
            Debug.Log("Failed to unload.");
            return;
        }
        var status = NetworkManager.Singleton.SceneManager.UnloadScene(sceneToUnload);
    }


}
