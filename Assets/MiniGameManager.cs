using System;
using System.Collections.Generic;
using Data;
using UnityEngine;


public enum MiniGameType
{
    PickUp,
    Digging,
    Fishing
}


public class MiniGameManager : MonoBehaviour
{
    
    public static MiniGameManager Instance;
    
    [SerializeField] 
    private List<MiniGame> miniGames;
    
    
    
    

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

    public void StartMiniGame(MiniGameType type, Action<bool> callback)
    {
        GameStateManager.Instance.SetGameState(GameStates.MiniGame);
        MiniGame miniGame = miniGames.Find(c => c.type == type);
        
        StartCoroutine(miniGame.StartMiniGame(
            (returnValue) =>
            {
                GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
                callback(returnValue);
            }
        ));
    }
}
