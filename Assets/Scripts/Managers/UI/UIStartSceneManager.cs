using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartSceneManager : MonoBehaviour
{
    [SerializeField] private Scenes sceneToLoad;
    public void StartGame()
    {
        GameStateManager.Instance?.ResumeGame();
        UIPauseMenuManager.Instance?.ResumeGame();
        StartCoroutine(SceneChangeManager.Instance.LoadSceneWithState(sceneToLoad));
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit Game");
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
