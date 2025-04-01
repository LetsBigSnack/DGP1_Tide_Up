using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Serialization;


public enum NpcStates
{
    Intro,
    QuestOffer,
    QuestInProgress,
    QuestCompleted
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
    
    
    private Dialogue _currentDialogue;   
    private List<Dialogue> _completedDialogues = new List<Dialogue>();
    private Quest _currentQuest;
    
    
    void Start()
    {
        _currentDialogue = DialogueManager.Instance.GetIntro(npcName);
    }

    public void InteractDialogue()
    {
        UIDialogueManager.Instance.SetDialogueBox(npcName, _currentDialogue.GetCurrentDialogue());
        
        CheckDialogueFinished();

    }

    public void ResetDialogue()
    {
        UIDialogueManager.Instance.ShowDialogueBox(false);
        _currentDialogue.ResetDialogue();
    }

    private void CheckDialogueFinished()
    {
        if (_currentDialogue == null)
        {
            throw new NullReferenceException();
        }

        if (_currentDialogue.IsDialogueFinished)
        {
            //TODO: this is only there cause as of now there isnt a deep copy of the dialgue, will later change with the addtion of quests
            _currentDialogue.IsDialogueFinished = false;
            switch (npcState)
            {
                case NpcStates.Intro:
                    npcState = NpcStates.QuestOffer;
                    
                    _completedDialogues.Add(_currentDialogue);
                    //TODO: remove this is just for dev needs to be removed later on
                    _currentDialogue = DialogueManager.Instance.GetRandomDialogueByPersonality(npcPersonality, npcAwareness);
                    _currentDialogue.CurrentDialogueState = 0;
                    break;
                case NpcStates.QuestOffer:
                    _completedDialogues.Add(_currentDialogue);
                    //TODO: remove this is just for dev needs to be removed later on
                    _currentDialogue = DialogueManager.Instance.GetRandomDialogueByPersonality(npcPersonality, npcAwareness);
                    _currentDialogue.CurrentDialogueState = 0;
                    break;
            }
            
        }
    }
}
