using System;
using System.Collections.Generic;
using Data;
using Helpers.Util;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

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
        Dialogue intro = new Dialogue(_dialogJsonData.Intros[npcName]);
        return intro;
    }

    public Dialogue GetRandomQuestDialogue(NpcPersonalities npcPersonality, NpcAwareness npcAwareness)
    {
        List<Dialogue> dialogues = _dialogJsonData.Quests[npcPersonality][npcAwareness];
        Dialogue quest = new Dialogue(dialogues[UnityEngine.Random.Range(0, dialogues.Count)]);
        return quest;
    }
    
    public Dialogue GetRandomProgressDialogue(NpcPersonalities npcPersonality, NpcAwareness npcAwareness)
    {
        List<Dialogue> dialogues = _dialogJsonData.InProgress[npcPersonality][npcAwareness];
        Dialogue progress = new Dialogue(dialogues[UnityEngine.Random.Range(0, dialogues.Count)]);
        return progress;
    }
    
    public Dialogue GetRandomCompleteDialogue(NpcPersonalities npcPersonality, NpcAwareness npcAwareness)
    {
        List<Dialogue> dialogues = _dialogJsonData.Complete[npcPersonality][npcAwareness];
        Dialogue complete = new Dialogue(dialogues[UnityEngine.Random.Range(0, dialogues.Count)]);
        return complete;
    }

    public Dialogue GetFinishedDialogByName(string npcName)
    {
        List<Dialogue> list = _dialogJsonData.Finished[npcName];
        Dialogue finished = new Dialogue(list[Random.Range(0, list.Count)]);
        return finished;
    }
}
