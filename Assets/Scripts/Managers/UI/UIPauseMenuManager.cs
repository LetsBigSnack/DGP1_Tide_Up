using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public enum PauseMenuType
{
    Paused,
    Options,
    SaveLoad
}

public class UIPauseMenuManager : MonoBehaviour
{
    [SerializeField] private string startSceneName;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private GameObject saveLoadMenu;

    private bool _isPaused = false;

    public static UIPauseMenuManager Instance;

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

    public void TogglePauseGame()
    {
        if (!_isPaused)
        {
            pauseMenu.SetActive(true);
            GameStateManager.Instance.SetGameState(GameStates.InMenu);
            _isPaused = true;
        }
        else 
        {
            pauseMenu.SetActive(false);
            GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
            _isPaused = false;
        }
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(startSceneName);
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
