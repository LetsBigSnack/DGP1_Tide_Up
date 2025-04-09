using System;
using System.Collections.Generic;
using Data;
using ScriptableObjects;
using TMPro;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;
    
    [SerializeField] private List<RecipeData> recipes;
    [SerializeField] private List<RecipeData> knowRecipes;
    
    
    
    public List<RecipeData> Recipes
    {
        get => recipes;
        set => recipes = value;
    }
    
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
    
    public QuestItemInstance ValidateRecipe(MaterialDetail materialDetail)
    {
        foreach (RecipeData recipe in recipes)
        {
            if (Equals(recipe.GetMaterialDetail(), materialDetail))
            {
                if (!knowRecipes.Contains(recipe))
                {
                    knowRecipes.Add(recipe);
                }
                return new QuestItemInstance(recipe.questItem, recipe.ingredients);
            }
        }

        return null;
    }

    public RecipeData GetRandomRecipe(QuestItemData questItem)
    {
        List<RecipeData> itemRecipes = recipes.FindAll(recipe => recipe.questItem == questItem);

        if (itemRecipes == null || itemRecipes.Count == 0)
        {
            throw new NullReferenceException();
        }
        
        return itemRecipes[UnityEngine.Random.Range(0, itemRecipes.Count)];
    }
}
