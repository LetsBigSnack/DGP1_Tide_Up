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
    Completed
}

public class Quest
{
    
    private Dialogue _questDialogue;
    private QuestItemData _questItemItem;
    private string _questItemUse;
    private List<string> _materials;
    private string _materialHint;
    private string _questNpc;
    private QuestState _questState;
    
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
    
    public Quest(Dialogue questDialogue)
    {
        _questState = QuestState.Offer;
        _questDialogue = questDialogue;
        
        InitializeQuestItem();
        GenerateMaterials();
        GenerateDialogueText();
    }
    
    private void InitializeQuestItem()
    {
        _questItemItem = DataUtil.Instance.GetRandomQuestItem();
        _questItemUse = _questItemItem.GetUse();
    }
    
    private void GenerateMaterials()
    {
        var shuffledMaterials = _questItemItem.materials
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
    }
    
    
    private Dictionary<string, string> GenerateKeywordReplacement()
    {
        return new Dictionary<string, string>
        {
            { "[QUEST-ITEM]", _questItemItem.name },
            { "[ITEM-REASON]", _questItemUse },
            { "[MATERIALS]", string.Join(", ", _materials) },
            { "[MATERIAL-HINT]", _materialHint }
        };
    }  
    
    public override string ToString()
    {
        return $"Quest State: {_questState}\n" +
               $"NPC: {_questNpc ?? "Unknown"}\n" +
               $"Quest Item: {_questItemItem?.name ?? "None"}\n" +
               $"Use: {_questItemUse ?? "None"}\n" +
               $"Materials Needed: {( _materials != null && _materials.Count > 0 ? string.Join(", ", _materials) : "None" )}\n" +
               $"Hint: {_materialHint ?? "None"}\n" +
               $"Dialogue:\n- {string.Join("\n- ", _questDialogue.DialogueContent)}";
    }

    public string GetCurrentDialogue()
    {
        string returnText = "";
        
        switch (_questState)
        {
            case QuestState.Offer:
                returnText = _questDialogue.GetCurrentDialogue();
                break;
            case QuestState.InProgress:
                if (CanQuestCoomplete())
                {
                    returnText = "Congratulations! You've completed the quest!";
                    //TODO: remove Inventory
                    
                }
                else
                {
                    returnText = "Quest in progress";
                }
                break;
            case QuestState.Completed:
                returnText = "Quest is complete";
                break;
        }
        return returnText;
    }

    private bool CanQuestCoomplete()
    {
        return false;
    }

    
    public void ResetDialogue()
    {
        _questDialogue.ResetDialogue();
    }


    public void NextDialogueContent()
    {
        _questDialogue.NextDialogueContent();
    }

    public bool IsDialogueComplete()
    {
        return _questDialogue.IsDialogueFinished;
    }

    public void AcceptQuest()
    {
        _questState = QuestState.InProgress;
    }

    public void DenyQuest()
    {
        _questDialogue.ResetDialogue();
    }
    
}
