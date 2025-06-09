using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;


public enum MiniGameType
{
    PickUp,
    Digging,
    Fishing,
    Boat
}

[Serializable]
public class MiniGameCooldown
{
    public MiniGameType Type;
    public float Cooldown;
}


public class MiniGameManager : MonoBehaviour
{
    
    public static MiniGameManager Instance;
    
    [SerializeField] 
    private List<MiniGame> miniGames;

    [SerializeField] private float miniGameCooldown = 0.1f;
    [SerializeField] private bool canPlayMinigame = true;
    
    [SerializeField] private List<MiniGameCooldown> minigameCooldowns = new List<MiniGameCooldown>();
    
    [SerializeField] private float elaspedTime;
    
    [SerializeField] private float _currentCooldown;
    

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


    private void FixedUpdate()
    {
        if (!canPlayMinigame)
        {
            elaspedTime += Time.deltaTime;
            if (elaspedTime >= _currentCooldown)
            {
                canPlayMinigame = true;
                elaspedTime = 0;
            }
        }
    }

    public bool CanPlayMinigame()
    {
        return canPlayMinigame;
    }
    
    
    public void StartMiniGame(MiniGameType type, Action<bool> callback)
    {
        if (!canPlayMinigame)
        {
            return;
        }
        
        GameStateManager.Instance.SetGameState(GameStates.MiniGame);
        MiniGame miniGame = miniGames.Find(c => c.type == type);
        AnimationController _animation = PlayerController.Instance.GetAnimationController();
        PlayerController.Instance?.SetPlayerCantMove();
    
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
            case MiniGameType.Boat:
                BoatController.Instance.AnchorBoat(true);
                break;
        }
        
        StartCoroutine(miniGame.StartMiniGame(
            (returnValue) =>
            {
                GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
                callback(returnValue);
                switch (type)
                {
                    case MiniGameType.PickUp:
                        PlayerController.Instance?.SetPlayerCanMove();
                        _animation?.DisableAnimation(Animations.Pick);
                        break;
                    case MiniGameType.Fishing:
                        _animation?.DisableAnimation(Animations.Fish);
                        break;
                    case MiniGameType.Digging:
                        _animation?.DisableAnimation(Animations.Dig);
                        break;
                    case MiniGameType.Boat:
                        PlayerController.Instance?.SetPlayerCanMove();
                        BoatController.Instance.AnchorBoat(false);
                        break;
                }
                _currentCooldown = minigameCooldowns.Where(c => c.Type == type).Select(c => c.Cooldown).FirstOrDefault();
                canPlayMinigame = false;
            }
            
            
        ));
    }
}
