using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NpcManager : MonoBehaviour
{
    
    public static NpcManager Instance;
    [SerializeField] private List<Npc> _npcs;

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
    
    public List<Npc> GetNpcs()
    {
        return _npcs;
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
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded");
        _npcs = new List<Npc>();
        _npcs = FindObjectsByType<Npc>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
    }
}
