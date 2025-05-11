using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NpcManager : MonoBehaviour
{
    
    public static NpcManager Instance;
    [SerializeField] private List<Npc> _npcs = new List<Npc>();
    [SerializeField] private List<NpcData> _npcsData = new List<NpcData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public NpcData GetNpcDataByName(string name)
    {
        return _npcsData.Find(npc => npc.NpcName == name);
    }

    public List<NpcData> GetNpcs()
    {
        return _npcsData;
    }

    public void AddNpc(Npc npc)
    {
        _npcs.Add(npc);
    }

    public NpcData GetNpcByName(string questNpc)
    {
        //Debug.Log(questNpc);
        NpcData npc = _npcsData.Find(npc => npc.NpcName == questNpc);

        if (npc == null)
        {
            throw new NullReferenceException();
        }
        return npc;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded");
        _npcs = new List<Npc>();
        _npcs = FindObjectsByType<Npc>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
    }
}
