using System;
using System.Collections.Generic;
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

    [SerializeField] private List<GameObject> borderIcon = new List<GameObject>();

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

    private void DeActivateBorders()
    {
        foreach(GameObject o in borderIcon)
        {
            o.SetActive(false);
        }
    }

    public void TogglePauseGame()
    {
        GameStateManager.Instance.TogglePause();
        if (!_isPaused)
        {
            DeActivateBorders();
            pauseMenu.SetActive(true);
            GameStateManager.Instance.SetGameState(GameStates.Paused);
            _pauseMenuState = PauseMenuStates.Paused;
            _isPaused = true;
            
        }
        else 
        {
            DeActivateBorders();
            pauseMenu.SetActive(false);
            GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
            _pauseMenuState = PauseMenuStates.Off;
            _isPaused = false;
        }
    }

    //TODO: Grenus Fix
    public void ResumeGame()
    {
        DeActivateBorders();
        pauseMenu.SetActive(false);
        GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
        _isPaused = false;
    }

    public void ToggleOptionMenu()
    {
        DeActivateBorders();
        if (_pauseMenuState == PauseMenuStates.Paused)
        {
            _pauseMenuState = PauseMenuStates.Options;
            optionMenu.SetActive(true);
            pauseMenu.SetActive(false);
            return;
        }

        _pauseMenuState = PauseMenuStates.Paused;
        optionMenu.SetActive(false);
        pauseMenu.SetActive(true);
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
