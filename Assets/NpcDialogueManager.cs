using System;
using UnityEngine;

public class NpcDialogueManager : MonoBehaviour
{
    public static NpcDialogueManager Instance;
    private Npc _currentNpc;
    
    
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
        _currentNpc = npc;
    }

    public void EndDialogue()
    {
        _currentNpc = null;
    }
    
    
}
