using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;


public enum NpcStates
{
    Intro,
    Quest,
    Finished
}

public enum NpcPersonalities
{
    Sporty,
    Nerdy,
    Hyper
}

public enum NpcAwareness
{
    Low,
    Medium,
    High
}

public class Npc : MonoBehaviour
{
    [Header("Current State")]
    [SerializeField] private NpcStates npcState = NpcStates.Intro;

    [Header("Npc Attributes")]
    [SerializeField] private NpcPersonalities npcPersonality;
    [SerializeField] private string npcName;
    [SerializeField] private string title;
    [SerializeField] private string homeDetails;
    [SerializeField] private string vibe;
    [SerializeField] private string birthday;
    [SerializeField] private string mbti;
    [SerializeField] private string description;
    [SerializeField] private string favColour;
    [SerializeField] private string favFood;
    [SerializeField] private string favAnimal;
    [SerializeField] private string favThing;
    [SerializeField] private Sprite portrait;

    [Header("Npc Awareness")]
    [SerializeField] private NpcAwareness npcAwareness = NpcAwareness.Low;
    [SerializeField] private int completedQuests = 0;
    [SerializeField] private int maxCompletedQuests = 6;

    [Header("Npc Island")]
    [SerializeField] private int islandID;
    
    
    public NpcStates NpcState
    {
        get => npcState;
        set => npcState = value;
    }

    public NpcPersonalities NpcPersonality
    {
        get => npcPersonality;
        set => npcPersonality = value;
    }
    public string NpcName
    {
        get => npcName;
        set => npcName = value;
    }

    public string Title
    {
        get => title;
        set => title = value;
    }

    public string HomeDetails
    {
        get => homeDetails;
        set => homeDetails = value;
    }

    public string Vibe
    {
        get => vibe;
        set => vibe = value;
    }

    public string Birthday
    {
        get => birthday;
        set => birthday = value;
    }

    public string Mbti
    {
        get => mbti;
        set => mbti = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    public string FavColour
    {
        get => favColour;
        set => favColour = value;
    }

    public string FavFood
    {
        get => favFood;
        set => favFood = value;
    }

    public string FavAnimal
    {
        get => favAnimal;
        set => favAnimal = value;
    }

    public string FavThing
    {
        get => favThing;
        set => favThing = value;
    }

    public Sprite Portrait
    {
        get => portrait;
        set => portrait = value;
    }

    public NpcAwareness NpcAwareness
    {
        get => npcAwareness;
        set => npcAwareness = value;
    }

    public int CompletedQuests
    {
        get => completedQuests;
        set => completedQuests = value;
    }

    public int MaxCompletedQuests
    {
        get => maxCompletedQuests;
        set => maxCompletedQuests = value;
    }

    public Dialogue CurrentDialogue
    {
        get => _currentDialogue;
        set => _currentDialogue = value;
    }
    
    public Dialogue FinishedDialogue
    {
        get => _finishedDialogue;
        set => _finishedDialogue = value;
    }

    public Quest CurrentQuest
    {
        get => _currentQuest;
        set => _currentQuest = value;
    }

    public int IslandID
    {
        get => islandID;
        set => islandID = value;
    }

    private Dialogue _currentDialogue;
    private Dialogue _finishedDialogue;
    private Quest _currentQuest;
    
    
    void Start()
    {
        _currentDialogue = DialogueManager.Instance.GetIntro(npcName);
        _currentQuest = QuestManager.Instance.GetQuestByName(npcName);
        if (_currentQuest != null)
        {
            _currentQuest.QuestNpc = npcName;
        }
    }

    public void CreateQuest()
    {
        npcState = NpcStates.Quest;
        InitializeQuest();
    }
    
    public void AddCompletedQuest()
    {
        completedQuests++;
        EnvironmentManager.Instance?.AddCleanlinessScore(EnvironmentActionType.Quest);
    }
    
    public void UpdateAwarness()
    {
        switch (completedQuests)
        {
            case <= 2:
                npcAwareness = NpcAwareness.Low;
                break;
            case <= 4:
                npcAwareness = NpcAwareness.Medium;
                break;
            case <= 6:
                npcAwareness = NpcAwareness.High;
                break;
        }
    }

    public bool HasMaxQuests()
    {
        bool hasMaxQuests = completedQuests >= maxCompletedQuests;

        if (hasMaxQuests)
        {
            npcState = NpcStates.Finished;
            _finishedDialogue = DialogueManager.Instance.GetFinishedDialogByName(npcName);
        }
        
        return hasMaxQuests;
    }

    public void AddQuest()
    {
        InitializeQuest();
    }

    private void InitializeQuest()
    {
        _currentQuest = QuestManager.Instance.CreateQuest(npcAwareness, npcPersonality);
        _currentQuest.QuestNpc = this.npcName;
    }

    public void AssignIsland(int islandID)
    {
        this.islandID = islandID;
    }
}
