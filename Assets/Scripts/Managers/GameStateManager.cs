using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    Playing,
    Paused
}


public class GameStateManager : MonoBehaviour
{
    [Range(0,1f)]
    [SerializeField] private float gameSpeed = 1f;
    [SerializeField] private GameStates _gameStates;
    private bool _gamePaused = false;

    public static GameStateManager Instance;
    
    
    
    
    

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            _gameStates = GameStates.Playing;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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

    private void PauseGame()
    {
        _gameStates = GameStates.Paused;
        _gamePaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    private void ResumeGame()
    {
        _gameStates = GameStates.Playing;
        _gamePaused = false;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }

    public void SetGameState(GameStates state)
    {
        _gameStates = state;
    }

    
}