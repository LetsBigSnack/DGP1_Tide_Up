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

    public event Action<DialogueChoice> OnChoiceChanged;

    public DialogueChoice CurrentChoice
    {
        get => currentChoice;
        set
        {
            if (currentChoice != value)
            {
                currentChoice = value;
                OnChoiceChanged?.Invoke(currentChoice);
            }
        }
    }

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
        GameStateManager.Instance.SetGameState(GameStates.Dialogue);
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
                HandelIntroState();
                break;
            case NpcStates.Quest:
                HandelQuestState();
                break;
            case NpcStates.Finished:
                HandelFinishedState();
                break;
        }
        
    }


    private void HandelIntroState()
    {
        
        if(TextToSpeechManager.Instance.IsTalking)
        {
            UIDialogueManager.Instance.FinishSpeaking();
            return;
        }
        
        if (_currentNpc.CurrentDialogue.IsDialogueFinished)
        {
            CloseDialogue();
            CheckDialogueFinished();
            return;
        }
        
        _currentNpc.CurrentDialogue.NextDialogueContent();
        UIDialogueManager.Instance?.SetDialogueBox(_currentNpc.NpcName, _currentNpc.CurrentDialogue.GetCurrentDialogue(), _currentNpc.FavColourCode);
        TextToSpeechManager.Instance?.TranslateTextToAudio(_currentNpc.CurrentDialogue.GetCurrentDialogue(), _currentNpc.Anim);
    }
    
    private void HandelQuestState()
    {
        if (TextToSpeechManager.Instance.IsTalking)
        {
            UIDialogueManager.Instance.FinishSpeaking();
            return;
        }

        if (isInChooseState)
        {
            MakeChoice(currentChoice);
        }
        else
        {
            if (_currentNpc.CurrentQuest.QuestState != QuestState.Offer &&
                _currentNpc.CurrentQuest.IsDialogueComplete())
            {
                if (_currentNpc.CurrentQuest.QuestState == QuestState.InProgress)
                {
                    ResetDialogue();
                }
                CloseDialogue();
                CheckDialogueFinished();
                return;
            }
                    
            _currentNpc.CurrentQuest.NextDialogueContent();
                    
            UIDialogueManager.Instance?.SetDialogueBox(_currentNpc.NpcName, _currentNpc.CurrentQuest.GetCurrentDialogue(), _currentNpc.FavColourCode);
            TextToSpeechManager.Instance?.TranslateTextToAudio(_currentNpc.CurrentQuest.GetCurrentDialogue(), _currentNpc.Anim);

            if (_currentNpc.CurrentQuest.IsDialogueComplete() && _currentNpc.CurrentQuest.QuestState == QuestState.Offer)
            {
                isInChooseState = true;
                UIDialogueManager.Instance?.ShowChoices(true);
            }
                    
        }
    }
    
    private void HandelFinishedState()
    {
        if (TextToSpeechManager.Instance.IsTalking)
        {
            UIDialogueManager.Instance.FinishSpeaking();
            return;
        }
        if (_currentNpc.FinishedDialogue.IsDialogueFinished)
        {
            ResetDialogue();
            return;
        }
                
        _currentNpc.FinishedDialogue.NextDialogueContent();
        UIDialogueManager.Instance?.SetDialogueBox(_currentNpc.NpcName, _currentNpc.FinishedDialogue.GetCurrentDialogue(), _currentNpc.FavColourCode);
        TextToSpeechManager.Instance?.TranslateTextToAudio(_currentNpc.FinishedDialogue.GetCurrentDialogue(), _currentNpc.Anim);

        CheckDialogueFinished();
    }

    

    private void CloseDialogue()
    {
        GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
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
                _currentNpc.FinishedDialogue = DialogueManager.Instance.GetFinishedDialogByName(_currentNpc.NpcName);
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
                    //TODO: Add sound
                    UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "New friendbook entry! " + _currentNpc.NpcName + " got added to your friendbook");
                    _currentNpc.CreateQuest();
                    ResetDialogue();
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
            if (!_currentNpc.HasMaxQuests())
            {
                _currentNpc.AddQuest();
                ResetDialogue();
            }
        }
    }
    
}
