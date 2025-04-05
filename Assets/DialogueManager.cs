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
        Dialogue dialogue = new Dialogue(_dialogJsonData.Intros[npcName]);
        return dialogue;
    }

    public Dialogue GetRandomDialogueByPersonality(NpcPersonalities npcPersonality, NpcAwareness npcAwareness)
    {
        List<Dialogue> dialogues = _dialogJsonData.Dialogues[npcPersonality][npcAwareness];
        Dialogue dialogue = new Dialogue(dialogues[UnityEngine.Random.Range(0, dialogues.Count)]);
        return dialogue;
    }
}
