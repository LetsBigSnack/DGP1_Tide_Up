using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class UpcycleManager : MonoBehaviour
{
    public static UpcycleManager Instance;

    private QuestItemInstance currentQuestItem;
    public static event Action<QuestItemInstance> OnQuestItemChanged;

    private List<TrashMaterialData> _ingredients;
    public List<TrashMaterialData> Ingredients
    {
        get => _ingredients;
        set => _ingredients = value;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _ingredients = new List<TrashMaterialData>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddIngredient(TrashMaterialData ingredient)
    {

        if (_ingredients.Count >= 4 || !InventoryManager.Instance.RemoveMaterial(ingredient.type, 1))
        {
            return false;
        }
        
        _ingredients.Add(ingredient);
        return true;
    }

    public bool RemoveIngredient(TrashMaterialData ingredient)
    {
        if (!_ingredients.Contains(ingredient) || !InventoryManager.Instance.AddMaterial(ingredient.type, 1))
        {
            return false;
        }
        _ingredients.Remove(ingredient);
        return true;
    }
    
    
    public bool RemoveAllIngredients()
    {
        MaterialDetail detail = new MaterialDetail(_ingredients);
        
        foreach (KeyValuePair<TrashMaterialType, int> ingredient in detail.MaterialDetails)
        {
            if (ingredient.Value == 0)
            {
                continue;
            }
            if (!InventoryManager.Instance.HasSpaceForMaterial(ingredient.Key, ingredient.Value))
            {
                return false;
            }
        }
        
        int count = _ingredients.Count;
        
        for (int i = 0; i < count; i++)
        {
            RemoveIngredient(_ingredients[0]);
        }
        
        return true;
    }


    private void ConsumeIngredients()
    {
        _ingredients = new List<TrashMaterialData>();
    }

    public QuestItemInstance ValidateRecipe()
    {
        MaterialDetail recipe = new MaterialDetail(_ingredients);
        return RecipeManager.Instance.ValidateRecipe(recipe);
    }

    public bool Upcycle()
    {
        if (!InventoryManager.Instance.HasSpaceForItem() || _ingredients.Count < 2)
        {
            return false;
        }
        
        currentQuestItem = ValidateRecipe();
        OnQuestItemChanged?.Invoke(currentQuestItem);

        if (currentQuestItem == null)
        {
            return false;
        }

        ConsumeIngredients();
        return true;
    }

    public void CollectQuestItem()
    {
        InventoryManager.Instance.AddItem(currentQuestItem);
        currentQuestItem = null;
        OnQuestItemChanged?.Invoke(currentQuestItem);
    }
    
    
}
