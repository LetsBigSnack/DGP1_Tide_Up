using System;
using UnityEngine;


public enum DialogueChoice
{
    Accept,
    Decline
}

public class NpcDialogueManager : MonoBehaviour
{
    public static NpcDialogueManager Instance;
    private Npc _currentNpc;
    [SerializeField] private bool isInChooseState = false;
    [SerializeField] private DialogueChoice currentChoice = DialogueChoice.Accept;

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
    
    public void StartDialogue(Npc npc)
    {
        if (npc == null || _currentNpc == npc)
        {
            return;
        }
        isInChooseState = false;
        _currentNpc = npc;
    }

    public void EndDialogue()
    {
        _currentNpc = null;
    }
    
    public void InteractDialogue()
    {
        if (_currentNpc == null)
        {
            return;
        }
        
        switch (_currentNpc.NpcState)
        {
            case NpcStates.Intro:
                _currentNpc.CurrentDialogue.NextDialogueContent();
                UIDialogueManager.Instance?.SetDialogueBox(_currentNpc.NpcName, _currentNpc.CurrentDialogue.GetCurrentDialogue());
                CheckDialogueFinished();
                break;
            case NpcStates.Quest:
                if (isInChooseState)
                {
                    MakeChoice(currentChoice);
                }
                else
                {
                    if (_currentNpc.CurrentQuest.QuestState != QuestState.Offer &&
                        _currentNpc.CurrentQuest.IsDialogueComplete())
                    {
                        CloseDialogue();
                        CheckDialogueFinished();
                        return;
                    }
                    
                    _currentNpc.CurrentQuest.NextDialogueContent();
                    
                    UIDialogueManager.Instance?.SetDialogueBox(_currentNpc.NpcName, _currentNpc.CurrentQuest.GetCurrentDialogue());
                
                    if (_currentNpc.CurrentQuest.IsDialogueComplete() && _currentNpc.CurrentQuest.QuestState == QuestState.Offer)
                    {
                        isInChooseState = true;
                        UIDialogueManager.Instance?.ShowChoices(true);
                    }
                    
                }
                break;
            case NpcStates.Finished:
                Debug.Log(_currentNpc.NpcName + ":" + "finished");
                break;
        }
        
    }

    private void CloseDialogue()
    {
        UIDialogueManager.Instance?.ShowDialogueBox(false);
        UIDialogueManager.Instance?.ShowChoices(false);
        isInChooseState = false;
    }

    public void MakeChoice(DialogueChoice dialogueChoice)
    {
        Debug.Log(_currentNpc.NpcName + ":" + dialogueChoice);
        switch (dialogueChoice)
        {
            case DialogueChoice.Accept:
                _currentNpc.CurrentQuest.AcceptQuest();
                break;
            case DialogueChoice.Decline:
                _currentNpc.CurrentQuest.DeclineQuest();
                break;
        }
        
        isInChooseState = false;
        ResetDialogue();
    }

    public void ResetDialogue()
    {
        if (_currentNpc == null)
        {
            return;
        }

        CloseDialogue();
        
        switch (_currentNpc.NpcState)
        {
            case NpcStates.Intro:
                _currentNpc.CurrentDialogue.ResetDialogue();
                break;
            case NpcStates.Quest:
                _currentNpc.CurrentQuest.ResetDialogue();
                break;
            case NpcStates.Finished:
                break;
        }
        
        _currentNpc = null;
    }

    private void CheckDialogueFinished()
    {
        if (_currentNpc == null)
        {
            return;
        }
        
        switch (_currentNpc.NpcState)
        {
            case NpcStates.Intro:
                if (_currentNpc.CurrentDialogue.IsDialogueFinished)
                {
                    _currentNpc.CreateQuest();
                }
                break;
            case NpcStates.Quest:
                CheckCompletedQuests();
                break;
        }
    }
    
    public void CheckCompletedQuests()
    {
        if (_currentNpc.CurrentQuest.QuestState == QuestState.Completed && _currentNpc.CurrentQuest.IsDialogueComplete())
        {
            _currentNpc.AddCompletedQuest();
            _currentNpc.UpdateAwarness();
            if (_currentNpc.HasMaxQuests())
            {
                _currentNpc.NpcState = NpcStates.Finished;
            }
            else
            {
                _currentNpc.AddQuest();
                ResetDialogue();
            }
        }
    }
    
}
