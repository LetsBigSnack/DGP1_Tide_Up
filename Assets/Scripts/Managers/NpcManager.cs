using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    
    public static NpcManager Instance;
    [SerializeField] private List<Npc> _npcs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void AddNpc(Npc npc)
    {
        _npcs.Add(npc);
    }

    public Npc GetNpcByName(string questNpc)
    {
        Debug.Log(questNpc);
        Npc npc = _npcs.Find(npc => npc.NpcName == questNpc);

        if (npc == null)
        {
            throw new NullReferenceException();
        }
        return npc;
    }
}
