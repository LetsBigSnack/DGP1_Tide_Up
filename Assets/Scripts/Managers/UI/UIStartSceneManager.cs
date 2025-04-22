using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartSceneManager : MonoBehaviour
{
    [SerializeField] private Scenes sceneToLoad;
    public void StartGame()
    {
        var player = Player.Instance;
        if (player != null)
        {
            Debug.Log("Before move: " + player.transform.position);
            player.transform.position += Vector3.up * 105f;
            Debug.Log("After move: " + player.transform.position);
        }
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
