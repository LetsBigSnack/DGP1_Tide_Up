using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum EnvironmentState
{
    Dirty,
    Clean,
    SuperClean
}

public enum EnvironmentActionType
{
    PickUp,
    Quest,
    Oil
}

[Serializable]
public class EnvironmentActionEntry
{ 
    [SerializeField] private EnvironmentActionType actionType;
    [SerializeField] private int score;
    
    public EnvironmentActionType ActionType
    {
        get => actionType;
        set => actionType = value;
    }

    public int Score
    {
        get => score;
        set => score = value;
    }
}

public class EnvironmentManager : MonoBehaviour
{    
    public static EnvironmentManager Instance;

    [Header("Environment Configuration")]
    [SerializeField] private List<EnvironmentActionEntry> actions;

    [Header("Island Management")]
    [SerializeField] private List<Island> islands;
    [SerializeField] private Island currentIsland;
    
    public static Action<EnvironmentState, int> OnEnvironmentStateChanged;
   
    public int TotalCleanlinessScore => islands.Sum(i => i.IslandCleanlinessScore);

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

    public List<Island> GetIslands()
    {
        return islands;
    }

    public void AddCleanlinessScore(EnvironmentActionType type)
    {
        
        EnvironmentActionEntry entry = actions.Find(x => x.ActionType == type);

        if (entry == null || currentIsland == null)
        {
            throw new NullReferenceException();
        }
        
        if (currentIsland.AddCleanlinessScore(entry.Score))
        {
            OnEnvironmentStateChanged?.Invoke(currentIsland.State, currentIsland.IslandID);
        }
    }
    
    
    public EnvironmentState GetStateOfIsland(int islandId)
    {
        
        Island island = islands.Find(i => i.IslandID == islandId);

        if (island == null)
        {
            throw new Exception($"Can't find island with id {islandId}");
        }
        
        return island.State;
    }

    public void SetCurrentIsland(Island island)
    {
        currentIsland = island;
        if (!island.HasVisited)
        {
            island.HasVisited = true;
        }
    }

    public void ClearCurrentIsland()
    {
        currentIsland = null;
    }

    public Island GetCurrentIsland()
    {
        return currentIsland;
    }
    public bool IsIslandUnlocked(int id)
    {
        return islands.Find(i => i.IslandID == id && i.HasVisited);
    }
}
