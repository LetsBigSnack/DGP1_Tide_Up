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
        AnimationController _animation = PlayerController.Instance.GetAnimationController();
        PlayerController.Instance?.SetPlayerCanMove(false);
    
        switch (type)
        {
            case MiniGameType.PickUp:
                _animation?.EnableAnimation(Animations.Pick);
                break;
            case MiniGameType.Digging:
                _animation?.EnableAnimation(Animations.Dig);
                break;
            case MiniGameType.Fishing:
                _animation?.EnableAnimation(Animations.Fish);
                break;
        }
        
        StartCoroutine(miniGame.StartMiniGame(
            (returnValue) =>
            {
                GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
                callback(returnValue);

                if (returnValue)
                {
                    switch (type)
                    {
                        case MiniGameType.PickUp:
                            PlayerController.Instance?.SetPlayerCanMove(true);
                            _animation?.DisableAnimation(Animations.Pick);
                            break;
                    }
                }
                
            }
        ));
    }
}
