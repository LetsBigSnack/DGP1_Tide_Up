using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class LocationManager : MonoBehaviour
{
    
    public static LocationManager Instance;
    
    [SerializeField] private float transitionWaitTime = 1f;
    [SerializeField] private Scenes currentScene;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            string activeSceneName = SceneManager.GetActiveScene().name;
            if (Enum.TryParse(activeSceneName, out Scenes parsedScene))
            {
                currentScene = parsedScene;
                Debug.Log($"Current scene set to: {currentScene}");
            }
            else
            {
                Debug.LogWarning($"Scene '{activeSceneName}' does not match any value in the Scenes enum.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void TravelToScene(Scenes targetScene)
    {
        StartCoroutine(TravelCoroutine(targetScene));
    }

    private IEnumerator TravelCoroutine(Scenes targetScene)
    {
 
        GameStateManager.Instance.SetGameState(GameStates.SceneTransition);
        yield return new WaitForSeconds(transitionWaitTime);

        // TODO: Play scene transition animation here
        Debug.Log("Play transition animation here.");
        
        bool sceneLoaded = false;
        yield return SceneChangeManager.Instance.LoadSceneWithState(targetScene, success => sceneLoaded = success);

        if (!sceneLoaded)
        {
            Debug.LogError("Scene failed to load. Aborting teleport.");
            GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
            yield break;
        }
        
        
        Scenes previousScene = currentScene;
        currentScene = targetScene;
        
        List<SpawnPoint> spawnPoints = GameObject.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None).ToList();
        foreach (var sp in spawnPoints)
        {
            if (sp.linkedScene == previousScene)
            {
                TeleportPlayer(sp.transform);
                Debug.Log("Player teleported to spawn point.");
                break;
            }
        }
        GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);

        yield return new WaitForSeconds(0.1f);
        //TODO: Add sound
        GameObject newToast = UI_ToastManager.Instance.CreateToast(UI_ToastManager.Instance.EnvironmentToastPrefab, UI_ToastManager.Instance.EnvironmentToastParent);
        newToast.GetComponent<ToastNotificationItem>().SetToast("Welcome to:", EnvironmentManager.Instance.CurrentIsland.IslandName);
    }

    private void TeleportPlayer(Transform targetPosition)
    {
        Player player = GameObject.FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.transform.position = targetPosition.position;
            player.transform.rotation = targetPosition.rotation; 
            Debug.Log("Player teleported to spawn point.");

        }
        else
        {
            Debug.LogError("No Player object found in scene.");
        }
    }
    
}
