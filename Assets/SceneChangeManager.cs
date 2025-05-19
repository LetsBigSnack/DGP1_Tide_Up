using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance;

    private bool isSceneChanging = false;
    private bool sceneFullyLoaded = false;
    private string targetSceneName = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        Debug.Log(scene);
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
        sceneFullyLoaded = false;
        targetSceneName = sceneName;

        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.sceneLoaded += HandleSceneLoaded;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = true;

        float timeout = 20f;
        float timer = 0f;

        while (!sceneFullyLoaded && timer < timeout)
        {
            Debug.Log($"Waiting for scene to fully load... Progress: {asyncLoad.progress}");
            
            //TOOD: fix
            if (asyncLoad.progress >= 0.9f)
            {
                onComplete?.Invoke(true);
                isSceneChanging = false;
            }
            
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (!sceneFullyLoaded)
        {
            Debug.LogWarning($"Scene load timeout exceeded for {sceneName}");
            onComplete?.Invoke(false);
        }
        else
        {
            Debug.Log($"Finished loading scene: {sceneName}");
            onComplete?.Invoke(true);
        }

        isSceneChanging = false;
        Debug.Log("Coroutine reached the end");
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == targetSceneName)
        {
            Debug.Log("SceneChangeManager detected scene loaded.");
            sceneFullyLoaded = true;
            UIFadeScreenHelper.Instance.EndTransition();

            //TOOD: talk with lucas about fix 
            isSceneChanging = false;
        }
    }

    public void ExitApplication()
    {
        Debug.Log("Exiting application.");
        Application.Quit();
    }

}
