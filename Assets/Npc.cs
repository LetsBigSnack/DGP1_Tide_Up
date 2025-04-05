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
    
    
    [SerializeField] private bool _isInChooseState = false;
    
    private Dialogue _currentDialogue;   
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

    public void InteractDialogue()
    {
        switch (npcState)
        {
            case NpcStates.Intro:
                _currentDialogue.NextDialogueContent();
                UIDialogueManager.Instance?.SetDialogueBox(npcName, _currentDialogue.GetCurrentDialogue());
                break;
            case NpcStates.Quest:

                if (_isInChooseState)
                {
                    
                }
                else
                {
                    _currentQuest.NextDialogueContent();
                }
                
                UIDialogueManager.Instance?.SetDialogueBox(npcName, _currentQuest.GetCurrentDialogue());
                
                if (_currentQuest.IsDialogueComplete())
                {
                    _isInChooseState = true;
                    UIDialogueManager.Instance?.ShowChoices(true);
                }
                break;
            case NpcStates.Finished:
                Debug.Log(npcName + ":" + "finished");
                break;
        }
        CheckDialogueFinished();
    }

    public void ResetDialogue()
    {
        UIDialogueManager.Instance?.ShowDialogueBox(false);
        UIDialogueManager.Instance?.ShowChoices(false);
        
        _isInChooseState = false;
        
        switch (npcState)
        {
            case NpcStates.Intro:
                _currentDialogue.ResetDialogue();
                break;
            case NpcStates.Quest:
                _currentQuest.ResetDialogue();
                break;
            case NpcStates.Finished:
                break;
        }
    }

    private void CheckDialogueFinished()
    {
        switch (npcState)
        {
            case NpcStates.Intro:
                if (_currentDialogue.IsDialogueFinished)
                {
                    npcState = NpcStates.Quest;
                    _currentQuest = QuestManager.Instance.CreateQuest(npcAwareness, npcPersonality);
                    _currentQuest.QuestNpc = this.npcName;
                }
                break;
            case NpcStates.Quest:
                if (_currentQuest.QuestState == QuestState.Completed)
                {
                    completedQuests++;
                    UpdateAwarness();
                    if (completedQuests >= maxCompletedQuests)
                    {
                        npcState = NpcStates.Finished;
                    }
                    else
                    {
                        _currentQuest = QuestManager.Instance.CreateQuest(npcAwareness, npcPersonality);
                        _currentQuest.QuestNpc = this.npcName;
                    }
                }
                break;
        }
    }

    private void UpdateAwarness()
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
}
