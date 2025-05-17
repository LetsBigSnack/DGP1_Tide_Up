using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartSceneManager : MonoBehaviour
{
    [SerializeField] private Scenes sceneToLoad;
    [SerializeField] private GameObject borderIcon;

    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject optionMenu;

    public GameObject BorderIcon
    {
        get { return borderIcon; }
        set { borderIcon = value; }
    }

    public static UIStartSceneManager Instance;

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

    public void OpenOptions()
    {
        optionMenu.SetActive(true);
        startMenu.SetActive(false);
        borderIcon.SetActive(false);
    }

    public void CloseOptions()
    {
        startMenu.SetActive(true);
        optionMenu.SetActive(false);
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
