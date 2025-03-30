using System;
using System.Collections.Generic;
using Data;
using Helpers.Util;
using Newtonsoft.Json;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    
    public static DialogueManager Instance;
    
    private DialogJsonData _dialogJsonData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _dialogJsonData = JSONUtil.GetDialogueData();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Dialogue GetIntro(string npcName)
    {
        if (_dialogJsonData?.Intros[npcName] == null)
        {
            throw new NullReferenceException();
        }
        return _dialogJsonData.Intros[npcName];
    }

    public Dialogue GetRandomDialogueByPersonality(NpcPersonalities npcPersonality, NpcAwareness npcAwareness)
    {
        List<Dialogue> dialogues = _dialogJsonData.Dialogues[npcPersonality][npcAwareness];
        
        return dialogues[UnityEngine.Random.Range(0, dialogues.Count)];
    }
}
