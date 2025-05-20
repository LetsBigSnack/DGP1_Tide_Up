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
    Hyper,
    Tutorial
}

public enum NpcAwareness
{
    Low,
    Medium,
    High
}

public class Npc : MonoBehaviour
{
    [SerializeField] private string npcName;
    [SerializeField] private NpcData data;
    private AnimationController _anim;

    public static event Action<int, Npc> OnCompletedQuests;

    public NpcData NpcData
    {
        get => data;
        set => data = value;
    }

    public NpcStates NpcState
    {
        get => data.NpcState;
        set => data.NpcState = value;
    }

    public NpcPersonalities NpcPersonality
    {
        get => data.NpcPersonality;
        set => data.NpcPersonality = value;
    }
    public string NpcName
    {
        get => data.NpcName;
        set => data.NpcName = value;
    }

    public string Title
    {
        get => data.Title;
        set => data.Title = value;
    }

    public string HomeDetails
    {
        get => data.HomeDetails;
        set => data.HomeDetails = value;
    }

    public string Vibe
    {
        get => data.Vibe;
        set => data.Vibe = value;
    }

    public string Birthday
    {
        get => data.Birthday;
        set => data.Birthday = value;
    }
    public int BirthDay
    {
        get => data.BirthDay;
        set => data.BirthDay = value;
    }
    public Seasons BirthSeason
    {
        get => data.BirthSeason;
        set => data.BirthSeason = value;
    }

    public string Mbti
    {
        get => data.Mbti;
        set => data.Mbti = value;
    }

    public string Description
    {
        get => data.Description;
        set => data.Description = value;
    }

    public string FavColour
    {
        get => data.FavColour;
        set => data.FavColour = value;
    }
    public Color FavColourCode
    {
        get => data.FavColourCode;
        set => data.FavColourCode = value;
    }

    public string FavFood
    {
        get => data.FavFood;
        set => data.FavFood = value;
    }

    public string FavAnimal
    {
        get => data.FavAnimal;
        set => data.FavAnimal = value;
    }

    public string FavThing
    {
        get => data.FavThing;
        set => data.FavThing = value;
    }

    public Sprite Portrait
    {
        get => data.Portrait;
        set => data.Portrait = value;
    }

    public NpcAwareness NpcAwareness
    {
        get => data.NpcAwareness;
        set => data.NpcAwareness = value;
    }

    public int CompletedQuests
    {
        get => data.CompletedQuests;
        set => data.CompletedQuests = value;
    }

    public int MaxCompletedQuests
    {
        get => data.MaxCompletedQuests;
        set => data.MaxCompletedQuests = value;
    }

    public int IslandID
    {
        get => data.IslandID;
        set => data.IslandID = value;
    }

    public Dialogue CurrentDialogue
    {
        get => data.CurrentDialogue;
        set => data.CurrentDialogue = value;
    }

    public Dialogue FinishedDialogue
    {
        get => data.FinishedDialogue;
        set => data.FinishedDialogue = value;
    }

    public Quest CurrentQuest
    {
        get => data.CurrentQuest;
        set => data.CurrentQuest = value;
    }

    public AnimationController Anim
    {
        get => _anim;
    }


    void Awake()
    {
        data = NpcManager.Instance.GetNpcDataByName(npcName);
        data.CurrentDialogue = DialogueManager.Instance.GetIntro(data.NpcName);
        data.CurrentQuest = QuestManager.Instance.GetQuestByName(data.NpcName);
        if (data.CurrentQuest != null)
        {
            data.CurrentQuest.QuestNpc = data.NpcName;
        }
        _anim = GetComponent<AnimationController>();
    }

    public void CreateQuest()
    {
        data.NpcState = NpcStates.Quest;
        InitializeQuest();
    }
    
    public void AddCompletedQuest()
    {
        if(data.NpcPersonality == NpcPersonalities.Tutorial)
        {
            data.CompletedQuests++;
        }
        data.CompletedQuests++;
        EnvironmentManager.Instance?.AddCleanlinessScore(EnvironmentActionType.Quest);
    }
    
    public void UpdateAwarness()
    {
        switch (data.CompletedQuests)
        {
            case <= 2:
                data.NpcAwareness = NpcAwareness.Low;
                break;
            case <= 4:
                data.NpcAwareness = NpcAwareness.Medium;
                break;
            case <= 6:
                data.NpcAwareness = NpcAwareness.High;
                break;
        }

        OnCompletedQuests?.Invoke(data.CompletedQuests, this);
    }

    public bool HasMaxQuests()
    {
        bool hasMaxQuests = data.CompletedQuests >= data.MaxCompletedQuests;

        if (hasMaxQuests)
        {
            data.NpcState = NpcStates.Finished;
            data.FinishedDialogue = DialogueManager.Instance.GetFinishedDialogByName(data.NpcName);
        }
        
        return hasMaxQuests;
    }

    public void AddQuest()
    {
        InitializeQuest();
    }

    public void InitializeQuest()
    {
        data.CurrentQuest = QuestManager.Instance.CreateQuest(data.NpcAwareness, data.NpcPersonality, data.NpcName, data.HomeDetails);
        data.CurrentQuest.QuestNpc = this.data.NpcName;
    }

    public void AssignIsland(int islandID)
    {
        this.data.IslandID = islandID;
    }
}
