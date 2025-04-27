using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public enum IslandObjectType
{
    Other,
    House,
    ReUpcycler,
    Shop,
    TideUpBox,
    Harbor
}

[Serializable]
public class IslandMilestone
{
    [SerializeField] private EnvironmentState state;
    [SerializeField] private int neededQuest;
    [SerializeField] private int neededScore;

    public EnvironmentState State
    {
        get => state;
        set => state = value;
    }

    public int NeededQuest
    {
        get => neededQuest;
        set => neededQuest = value;
    }

    public int NeededScore
    {
        get => neededScore;
        set => neededScore = value;
    }
}

public class Island : MonoBehaviour
{
    [Header("Island Info")]
    [SerializeField] private int islandID;
    [SerializeField] private string islandName;
    [SerializeField] private bool hasVisited;
    [SerializeField] private Vector3 islandCenter;

    [Header("Cleanliness")]
    [SerializeField] private int islandCleanlinessScore = 0;
    [SerializeField] private List<IslandMilestone> milestones = new List<IslandMilestone>();
    [SerializeField] private EnvironmentState state = EnvironmentState.Dirty;

    [Header("Detection")]
    [SerializeField] private float islandRadius = 100f;
    [SerializeField] private LayerMask layerMasks;

    [Header("NPCs")]
    [SerializeField] private int maxNpcNumber;
    [SerializeField] private List<Npc> npcs = new List<Npc>();

    [Header("Gizmos")]
    [SerializeField] private Color gizmoColor = Color.yellow;
    
    private SphereCollider _sphereCollider;
    private HashSet<GameObject> _player = new HashSet<GameObject>();
    private List<IslandObjectType> _objectsOnIsland = new List<IslandObjectType>();
    
    
    public int IslandID
    {
        get => islandID;
        set => islandID = value;
    }
    
    public string IslandName
    {
        get => islandName;
        set => islandName = value;
    }

    public bool HasVisited
    {
        get => hasVisited;
        set => hasVisited = value;
    }

    public int IslandCleanlinessScore
    {
        get => islandCleanlinessScore;
        set => islandCleanlinessScore = value;
    }

    public Vector3 IslandCenter
    {
        get => islandCenter;
    }

    public List<IslandObjectType> ObjectsOnIsland
    {
        get => _objectsOnIsland;
    }

    public EnvironmentState State
    {
        get => state;
        set => state = value;
    }

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.radius = islandRadius;
        islandCenter = _sphereCollider.center;
        milestones.Sort((x, y) => x.State.CompareTo(y.State));
        AssignAllEnvironment();
    }

    private void AssignAllEnvironment()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, islandRadius, layerMasks);
        
        foreach (Collider hit in hits)
        {
            EnvironmentObject environmentObject = hit.GetComponentInParent<EnvironmentObject>();
            Npc npc = hit.GetComponentInParent<Npc>();
            if (environmentObject != null)
            {
                environmentObject.AssignIsland(islandID);
                if (!_objectsOnIsland.Contains(environmentObject.Type) && environmentObject.Type != IslandObjectType.Other)
                {
                    AddObjectTypesToIsland(environmentObject);
                }
            }

            if (npc != null)
            {
                npcs.Add(npc);
                npc.AssignIsland(IslandID);
            }
        }
    }

    private void AddObjectTypesToIsland(EnvironmentObject eObject)
    {
        _objectsOnIsland.Add(eObject.Type);
    }

    public bool AddCleanlinessScore(int score)
    {
        islandCleanlinessScore += score;
        
        return CheckState();
    }

    private bool CheckState()
    {
        int completedQuests = npcs.Sum(c => c.CompletedQuests);
        bool changedState = false;

        foreach (IslandMilestone milestone in milestones)
        {
            if (islandCleanlinessScore < milestone.NeededScore || completedQuests < milestone.NeededQuest)
            {
                break;
            }

            if (milestone.State == state)
            {
                continue;
            }
            state = milestone.State;
            changedState = true;
        }
        
        return changedState;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_player.Contains(other.transform.root.gameObject))
        {
            return;
        }
        _player.Add(other.transform.root.gameObject);
        EnvironmentManager.Instance.SetCurrentIsland(this);
    }


    private void OnTriggerExit(Collider other)
    {
        if (!_player.Contains(other.transform.root.gameObject))
        {
            return;
        }
        _player.Remove(other.transform.root.gameObject);
        EnvironmentManager.Instance.ClearCurrentIsland();
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, islandRadius);
    }
    
}
