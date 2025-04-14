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
    [SerializeField] private NpcStates npcState = NpcStates.Intro;
    [SerializeField] private NpcPersonalities npcPersonality;
    [SerializeField] private string npcName;
    [SerializeField] private NpcAwareness npcAwareness = NpcAwareness.Low;
    [SerializeField] private int completedQuests = 0;
    [SerializeField] private int maxCompletedQuests = 6;
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
        NpcManager.Instance.AddNpc(this);
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
