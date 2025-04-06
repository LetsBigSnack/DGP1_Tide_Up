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
    private QuestItemData _questItem;
    private string _questItemUse;
    private List<string> _materials;
    private string _materialHint;
    private string _questNpc;
    private QuestState _questState;
    private bool _hasQuestAccepted;


    public string QuestNpc
    {
        get { return _questNpc; }
        set { _questNpc = value; }
    }

    public QuestState QuestState
    {
        get { return _questState; }
        set { _questState = value; }
    }
    
    public Quest(Dialogue questDialogue, Dialogue completedDialogue)
    {
        _questState = QuestState.Offer;
        
        _dialogues.Add(QuestState.Offer, questDialogue);
        _dialogues.Add(QuestState.Completed, completedDialogue);
        
        InitializeQuestItem();
        GenerateMaterials();
        GenerateDialogueText();
        
        _dialogues.Add(QuestState.InProgress, null);
    }
    
    private void InitializeQuestItem()
    {
        _questItem = DataUtil.Instance.GetRandomQuestItem();
        _questItemUse = _questItem.GetUse();
    }
    
    private void GenerateMaterials()
    {
        var shuffledMaterials = _questItem.materials
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
    
    
    private Dictionary<string, string> GenerateKeywordReplacement()
    {
        return new Dictionary<string, string>
        {
            { "[QUEST-ITEM]", _questItem.name },
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
        Npc npc = NpcManager.Instance.GetNpcByName(_questNpc);
        _dialogues[QuestState.InProgress] = DialogueManager.Instance.GetRandomProgressDialogue(npc.NpcPersonality,npc.NpcAwareness);
        _hasQuestAccepted = true;
    }

    public void DeclineQuest()
    {
        _dialogues[QuestState.Offer].ResetDialogue();
    }
    
}
