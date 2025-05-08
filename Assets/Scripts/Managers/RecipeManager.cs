using System;
using System.Collections.Generic;
using Data;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using System.Linq;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;
    
    [SerializeField] private List<RecipeData> recipes;
    [SerializeField] private List<RecipeData> knowRecipes;

    public static event Action<bool> OnKnownRecipesChanged;
    
    public List<RecipeData> Recipes
    {
        get => recipes;
        set => recipes = value;
    }

    public List<RecipeData> KnownRecipies
    {
        get => knowRecipes;
        set => knowRecipes = value;
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
                    OnKnownRecipesChanged?.Invoke(true);

                    //TODO: Add sound
                    GameObject newToast = UI_ToastManager.Instance.CreateToast(UI_ToastManager.Instance.ImportantToastPrefab, UI_ToastManager.Instance.ImportantToastParent);
                    newToast.GetComponent<ToastNotificationItem>().SetToast("You unlucked a new recipe: " + recipe.questItem.title, " ");
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
