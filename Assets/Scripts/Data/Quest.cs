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
    //TODO pack everything into one thing.
    private Dialogue _questDialogue;
    private Dialogue _progressDialogue;
    private Dialogue _completedDialogue;
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
        _questDialogue = questDialogue;
        _completedDialogue = completedDialogue;
        
        InitializeQuestItem();
        GenerateMaterials();
        GenerateDialogueText();
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
        
        for (int i = 0; i < _questDialogue.DialogueContent.Count; i++)
        {
            string line = _questDialogue.DialogueContent[i];
            foreach (KeyValuePair<string, string> pair in replacements)
            {
                line = line.Replace(pair.Key, pair.Value);
            }
            _questDialogue.DialogueContent[i] = line;
        }
        
        for (int i = 0; i < _completedDialogue.DialogueContent.Count; i++)
        {
            string line = _completedDialogue.DialogueContent[i];
            foreach (KeyValuePair<string, string> pair in replacements)
            {
                line = line.Replace(pair.Key, pair.Value);
            }
            _completedDialogue.DialogueContent[i] = line;
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
    
    public override string ToString()
    {
        return $"Quest State: {_questState}\n" +
               $"NPC: {_questNpc ?? "Unknown"}\n" +
               $"Quest Item: {_questItem?.name ?? "None"}\n" +
               $"Use: {_questItemUse ?? "None"}\n" +
               $"Materials Needed: {( _materials != null && _materials.Count > 0 ? string.Join(", ", _materials) : "None" )}\n" +
               $"Hint: {_materialHint ?? "None"}\n" +
               $"Dialogue:\n- {string.Join("\n- ", _questDialogue.DialogueContent)}";
    }

    public string GetCurrentDialogue()
    {
        if (CanQuestComplete())
        {
            _questState = QuestState.Completed;
        }
        
        string returnText = "";
        
        switch (_questState)
        {
            case QuestState.Offer:
                returnText = _questDialogue.GetCurrentDialogue();
                break;
            case QuestState.InProgress:
                returnText = _progressDialogue.GetCurrentDialogue();
                break;
            case QuestState.Completed:
                returnText = _completedDialogue.GetCurrentDialogue();
                break;
        }
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

        switch (_questState)
        {
            case QuestState.Offer:
                _questDialogue.ResetDialogue();
                break;
            case QuestState.InProgress:
                _progressDialogue.ResetDialogue();
                break;
            case QuestState.Completed:
                _completedDialogue.ResetDialogue();
                break;
        }
    }


    public void NextDialogueContent()
    {
        switch (_questState)
        {
         case QuestState.Offer:
             _questDialogue.NextDialogueContent();
             break;
         case QuestState.InProgress:
             _progressDialogue.NextDialogueContent();
             break;
         case QuestState.Completed:
             _completedDialogue.NextDialogueContent();
             break;
        }
        
        
    }

    public bool IsDialogueComplete()
    {

        switch (_questState)
        {
            case QuestState.Offer:
                return _questDialogue.IsDialogueFinished;
            case QuestState.InProgress:
                return _progressDialogue.IsDialogueFinished;
            case QuestState.Completed:
                return _completedDialogue.IsDialogueFinished;
        }
        
        return _questDialogue.IsDialogueFinished;
    }

    public void AcceptQuest()
    {
        _questState = QuestState.InProgress;
        Npc npc = NpcManager.Instance.GetNpcByName(_questNpc);
        _progressDialogue = DialogueManager.Instance.GetRandomProgressDialogue(npc.NpcPersonality,npc.NpcAwareness);
        _hasQuestAccepted = true;
    }

    public void DeclineQuest()
    {
        _questDialogue.ResetDialogue();
    }
    
}
