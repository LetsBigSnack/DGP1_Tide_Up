using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataUtil : MonoBehaviour
{
    public static DataUtil Instance;
    
    [SerializeField] private List<TrashData> listOfTrash = new();
    [SerializeField] private List<TrashMaterialData> listOfMaterials = new();
    [SerializeField] private List<QuestItemData> listOfItems = new List<QuestItemData>();
    
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public TrashData GetRandomTrash()
    {
        return listOfTrash[UnityEngine.Random.Range(0, listOfTrash.Count)];
    }

    public List<TrashMaterialData> GetMaterials()
    {
        return listOfMaterials;
    }
    
    public QuestItemData GetRandomQuestItem()
    {
        return listOfItems[UnityEngine.Random.Range(0, listOfItems.Count)];
    }
    

    public TrashMaterialData GetMaterialByType(TrashMaterialType type)
    {
        return listOfMaterials.Where(t => t.type == type).FirstOrDefault();
    }
}
