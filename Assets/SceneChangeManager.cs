using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    BE_155_Location_Manager,
    Dev_Room_1,
    Dev_Room_2
}

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance;

    private bool isSceneChanging = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ChangeScene(Scenes scene)
    {
        StartCoroutine(LoadSceneWithState(scene, null));
    }
    
    public IEnumerator LoadSceneWithState(Scenes scene, Action<bool> onComplete = null)
    {
        if (isSceneChanging)
        {
            Debug.Log("Scene change is already in progress.");
            onComplete?.Invoke(false);
            yield break;
        }

        string sceneName = scene.ToString();

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' cannot be loaded. Check build settings.");
            onComplete?.Invoke(false);
            yield break;
        }

        isSceneChanging = true;

        Debug.Log($"Loading scene: {sceneName}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log($"Finished loading scene: {sceneName}");
        isSceneChanging = false;

        onComplete?.Invoke(true);
    }

    public void ExitApplication()
    {
        Debug.Log("Exiting application.");
        Application.Quit();
    }

    private void OnEnable()
    {
        isSceneChanging = false;
    }
}
