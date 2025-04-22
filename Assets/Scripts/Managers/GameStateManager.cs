using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    PlayingCharacter,
    PlayingBoat,
    Paused,
    InMenu,
    SceneTransition,
    Dialogue
}


public class GameStateManager : MonoBehaviour
{
    [Range(0,1f)]
    [SerializeField] private float gameSpeed = 1f;
    [SerializeField] private GameStates _gameStates;
    private bool _gamePaused = false;

    public static GameStateManager Instance;

    public static Action<GameStates> OnStateChanged;

    
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            _gameStates = GameStates.PlayingCharacter;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        OnStateChanged.Invoke(_gameStates);
    }

    private void FixedUpdate()
    {
        Time.timeScale = gameSpeed;
    }

    public GameStates GetGameState()
    {
        return _gameStates;
    }

    public void TogglePause()
    {
        if (_gamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        _gameStates = GameStates.Paused;
        _gamePaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
        OnStateChanged.Invoke(_gameStates);
    }

    public void ResumeGame()
    {
        _gameStates = GameStates.PlayingCharacter;
        _gamePaused = false;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
        OnStateChanged.Invoke(_gameStates);
    }

    public void SetGameState(GameStates state)
    {
        Debug.Log("Game state set to: " + state);
        _gameStates = state;
        OnStateChanged.Invoke(state);
    }

    
}