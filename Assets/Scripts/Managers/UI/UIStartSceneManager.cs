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
                
                GameObject player = Player.Instance?.gameObject;

                if (player != null)
                {
                    MeshRenderer[] meshes = player.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mesh in meshes)
                    {
                        if (mesh != null)
                            mesh.enabled = true;
                    }
                }
                

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
