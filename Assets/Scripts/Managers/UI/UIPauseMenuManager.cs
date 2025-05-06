using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PauseMenuStates
{
    Off,
    Paused,
    Options,
    SaveLoad
}

public class UIPauseMenuManager : MonoBehaviour
{
    [SerializeField] private Scenes startScene;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private GameObject saveLoadMenu;

    [SerializeField] private Toggle tooltipToggle;

    public static event Action<Toggle> OnTooltipToggleChange;

    private PauseMenuStates _pauseMenuState = PauseMenuStates.Off;

    private bool _isPaused = false;

    public PauseMenuStates PauseMenuState
    {
        get { return _pauseMenuState; }
        set { _pauseMenuState = value; }
    }

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
        GameStateManager.Instance.TogglePause();
        if (!_isPaused)
        {
            pauseMenu.SetActive(true);
            GameStateManager.Instance.SetGameState(GameStates.Paused);
            _pauseMenuState = PauseMenuStates.Paused;
            _isPaused = true;
            
        }
        else 
        {
            pauseMenu.SetActive(false);
            GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
            _pauseMenuState = PauseMenuStates.Off;
            _isPaused = false;
        }
    }

    //TODO: Grenus Fix
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
        _isPaused = false;
    }

    public void ToggleOptionMenu()
    {
        if(_pauseMenuState == PauseMenuStates.Paused)
        {
            _pauseMenuState = PauseMenuStates.Options;
            optionMenu.SetActive(true);
            return;
        }

        _pauseMenuState = PauseMenuStates.Paused;
        optionMenu.SetActive(false);
    }

    public void ToggleTooltip()
    {
        OnTooltipToggleChange?.Invoke(tooltipToggle);
    }

    public void ToMainMenu()
    {
        StartCoroutine(SceneChangeManager.Instance.LoadSceneWithState(startScene));
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
