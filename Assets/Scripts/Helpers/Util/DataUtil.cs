using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Data;
using ScriptableObjects;

public class DataUtil : MonoBehaviour
{
    public static DataUtil Instance;
    
    [SerializeField] private List<TrashData> listOfTrash = new();
    [SerializeField] private List<TrashMaterialData> listOfMaterials = new();
    [SerializeField] private List<QuestItemData> listOfItems = new List<QuestItemData>();
    [SerializeField] private List<QuestItemData> listOfTutorialItems = new List<QuestItemData>();

    public void Awake()
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

    public TrashItemInstance GetRandomTrash()
    {
        TrashItemInstance trashItem = new TrashItemInstance(listOfTrash[UnityEngine.Random.Range(0, listOfTrash.Count)], false);
        
        return trashItem;
    }

    public List<TrashMaterialData> GetMaterials()
    {
        return listOfMaterials;
    }
    
    public QuestItemInstance GetRandomQuestItem()
    {
        QuestItemData questItem = listOfItems[UnityEngine.Random.Range(0, listOfItems.Count)];
        RecipeData recipe = RecipeManager.Instance.GetRandomRecipe(questItem);
        
        return new QuestItemInstance(questItem, recipe.ingredients);
    }
    

    public TrashMaterialData GetMaterialByType(TrashMaterialType type)
    {
        return listOfMaterials.FirstOrDefault(t => t.type == type);
    }

    public QuestItemInstance GetTutorialItem()
    {
        QuestItemData questItem = listOfTutorialItems[UnityEngine.Random.Range(0, listOfItems.Count)];
        RecipeData recipe = RecipeManager.Instance.GetRandomRecipe(questItem);

        return new QuestItemInstance(questItem, recipe.ingredients);
    }
}
