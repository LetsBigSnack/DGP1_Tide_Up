using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Unity.VisualScripting;
using UnityEngine;


public enum QuestState
{
    Offer,
    InProgress,
    Completed,
}

public class Quest
{
    private Dictionary<QuestState, Dialogue> _dialogues = new Dictionary<QuestState, Dialogue>();
    private QuestItemInstance _questItem;
    private string _questItemUse;
    private List<string> _materials;
    private string _materialHint;
    private string _questNpc;
    private QuestState _questState;
    private bool _hasQuestAccepted;
    private string _questTitle;
    private string _questLocation;

    public static event Action<QuestState, string> OnQuestStateChanged;

    public string QuestNpc
    {
        get { return _questNpc; }
        set { _questNpc = value; }
    }

    public QuestItemInstance QuestItem
    {
        get { return _questItem; }
        set { _questItem = value; }
    }


    public QuestState QuestState
    {
        get { return _questState; }
        set { _questState = value; }
    }

    public string QuestTitle
    {
        get { return _questTitle; }
        set { _questTitle = value; }
    }
    public string QuestLocation
    {
        get { return _questLocation; }
        set { _questLocation = value; }
    }

    public Dictionary<QuestState, Dialogue> Dialogues
    {
        get { return _dialogues; }
    }

    public Quest(Dialogue questDialogue, Dialogue completedDialogue, NpcPersonalities npcPersonality, string npcName, string location)
    {
        _questTitle = npcName + "'s quest";
        _questLocation = location;
        _questState = QuestState.Offer;
        OnQuestStateChanged?.Invoke(_questState, _questNpc);
        
        _dialogues.Add(QuestState.Offer, questDialogue);
        _dialogues.Add(QuestState.Completed, completedDialogue);
        
        if(npcPersonality != NpcPersonalities.Tutorial)
        {
            InitializeQuestItem();
        } 
        else
        {
            InitializeTutorialtItem();
        }
        GenerateMaterials();
        GenerateDialogueText();
        
        _dialogues.Add(QuestState.InProgress, null);
    }
    
    private void InitializeQuestItem()
    {
        _questItem = DataUtil.Instance.GetRandomQuestItem();
        _questItemUse = _questItem.GetUse();
    }

    private void InitializeTutorialtItem()
    {
        _questItem = DataUtil.Instance.GetTutorialItem();
        _questItemUse = _questItem.GetUse();
    }

    //TODO: rework with recipes
    private void GenerateMaterials()
    {
        var shuffledMaterials = _questItem.GetMaterials()
            .OrderBy(_ => Guid.NewGuid())
            .ToList();

        _materialHint = shuffledMaterials[0].GetHint();
        _materials = shuffledMaterials
            .Skip(1)
            .Select(m => m.materialName)
            .ToList();
    }

    private void GenerateDialogueText()
    {
        Dictionary<string, string> replacements = GenerateKeywordReplacement();

        foreach (KeyValuePair<QuestState, Dialogue> dialogue in _dialogues)
        {
            Dialogue tempDialogue = dialogue.Value;
            
            if (tempDialogue == null)
            {
                continue;
            }
            
            for (int i = 0; i < tempDialogue.DialogueContent.Count; i++)
            {
                string line = tempDialogue.DialogueContent[i];
                foreach (KeyValuePair<string, string> pair in replacements)
                {
                    line = line.Replace(pair.Key, pair.Value);
                }
                tempDialogue.DialogueContent[i] = line;
            }
        }
    }

    private void CreateSubTasks()
    {
        //TODO: create subtasks to keep track of
    }
    
    
    private Dictionary<string, string> GenerateKeywordReplacement()
    {
        return new Dictionary<string, string>
        {
            { "[QUEST-ITEM]", _questItem.ItemData.name },
            { "[ITEM-REASON]", _questItemUse },
            { "[MATERIALS]", string.Join(", ", _materials) },
            { "[MATERIAL-HINT]", _materialHint }
        };
    }  
    
    public string GetCurrentDialogue()
    {
        if (CanQuestComplete())
        {
            _questState = QuestState.Completed;
            OnQuestStateChanged?.Invoke(QuestState.Completed, _questNpc);
        }
        
        string returnText = "";
        
        returnText = _dialogues[_questState]?.GetCurrentDialogue();
        
        return returnText;
    }

    private bool CanQuestComplete()
    {

        if (!InventoryManager.Instance.HasItem(_questItem) || !_hasQuestAccepted)
        {
            return false;
        }
        InventoryManager.Instance.RemoveItem(_questItem);
        QuestManager.Instance.CompleteQuest(this);
        return true;
    }

    
    public void ResetDialogue()
    {
        _dialogues[_questState]?.ResetDialogue();
    }


    public void NextDialogueContent()
    {
        _dialogues[_questState]?.NextDialogueContent();
    }

    public bool IsDialogueComplete()
    {
        if (_dialogues[_questState] == null)
        {
            return false;
        }
        
        return  _dialogues[_questState].IsDialogueFinished;
    }

    public void AcceptQuest()
    {
        _questState = QuestState.InProgress;
        OnQuestStateChanged?.Invoke(QuestState.InProgress, _questNpc);
        NpcData npc = NpcManager.Instance.GetNpcByName(_questNpc);
        _dialogues[QuestState.InProgress] = DialogueManager.Instance.GetRandomProgressDialogue(npc.NpcPersonality,npc.NpcAwareness);
        _hasQuestAccepted = true;

        QuestManager.Instance.CurrentQuests.Add(this);
    }

    public void DeclineQuest()
    {
        _dialogues[QuestState.Offer].ResetDialogue();
    }
    
}
