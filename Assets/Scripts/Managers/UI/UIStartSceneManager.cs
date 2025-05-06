using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartSceneManager : MonoBehaviour
{
    [SerializeField] private Scenes sceneToLoad;
    public void StartGame()
    {
        StartCoroutine(SceneChangeManager.Instance.LoadSceneWithState(sceneToLoad,loaded =>
        {
            if (loaded)
            {
                GameStateManager.Instance?.SetGameState(GameStates.PlayingCharacter);
                GameStateManager.Instance?.ResumeGame();
                UIPauseMenuManager.Instance?.ResumeGame();
            }
        }));
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
