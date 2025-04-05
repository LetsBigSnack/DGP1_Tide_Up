using System.Collections.Generic;
using Data;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    
    private List<Quest> _currentQuests;
    private List<Quest> _completedQuests;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _currentQuests = new List<Quest>();
            _completedQuests = new List<Quest>();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public Quest GetQuestByName(string npcName)
    {
        Quest quest = _currentQuests.Find(q => q.QuestNpc == npcName);
        return quest;
    }


    public Quest CreateQuest(NpcAwareness npcAwareness, NpcPersonalities npcPersonalities)
    {
        Dialogue questDialogue = DialogueManager.Instance.GetRandomQuestDialogue(npcPersonalities, npcAwareness);
        Quest quest = new Quest(questDialogue);
        return quest;
    }

}
