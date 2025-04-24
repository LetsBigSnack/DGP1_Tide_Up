using Assets.Scripts.Data;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

public class UIRecipeController : UIJournalSubMenu
{
    public static UIRecipeController Instance;

    [Header("SubMenu")]
    [SerializeField] private GameObject leftPage;
    [SerializeField] private GameObject rightPage;

    [Header("UIRecipeItems")]
    [SerializeField] private GameObject recipePrefab;
    [SerializeField] private Transform recipeParent;

    [Header("UIUpcyclerRecipeItem")]
    [SerializeField] private GameObject upcyclerPrefab;
    [SerializeField] private UIUpcyclerRecipeEntryItem currentOpenItem;

    private Dictionary<QuestItemData, List<RecipeData>> _knownRecipies = new Dictionary<QuestItemData, List<RecipeData>>();
    private List<GameObject> curEntries = new List<GameObject>();

    private void OnEnable()
    {
        RecipeManager.OnKnownRecipesChanged += CreateRecipies;
    }

    private void OnDisable()
    {
        RecipeManager.OnKnownRecipesChanged -= CreateRecipies;
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

    public override void CloseMenu()
    {
        leftPage.SetActive(false);
        rightPage.SetActive(false);
        UIInventoryController.Instance.CloseWallet();
    }

    public override void OpenMenu()
    {
        if(UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Upcycler)
        {
            leftPage.SetActive(true);
            rightPage.SetActive(false);
            UIInventoryController.Instance.OpenWallet();
        }
        else
        {
            leftPage.SetActive(true);
            rightPage.SetActive(true);
        }
        CreateRecipies();
    }

    private void UpdateRecipies()
    {
        List<RecipeData> curKnownRecipies = RecipeManager.Instance.KnownRecipies;

        foreach(RecipeData r in curKnownRecipies)
        {
            if (!_knownRecipies.ContainsKey(r.questItem))
            {
                _knownRecipies.Add(r.questItem, new List<RecipeData>());
                _knownRecipies[r.questItem].Add(r);
            }
            else
            {
                if (!_knownRecipies[r.questItem].Find(x => x == r))
                {
                    _knownRecipies[r.questItem].Add(r);
                }
            }
        }
    }

    private void CreateRecipies(bool refresh = false)
    {
        UpdateRecipies();
        ClearEntries();

        if(UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Upcycler)
        {
            foreach (QuestItemData q in _knownRecipies.Keys)
            {
                GameObject newUpcyclerEntry = Instantiate(upcyclerPrefab, recipeParent);
                newUpcyclerEntry.GetComponentInChildren<UIUpcyclerRecipeEntryItem>().Setup(q, _knownRecipies[q]);
                curEntries.Add(newUpcyclerEntry);
            }
            return;
        }

        foreach(QuestItemData q in _knownRecipies.Keys)
        {
            GameObject newRecipeEntry = Instantiate(recipePrefab, recipeParent);
            newRecipeEntry.GetComponent<UIRecipeEntryItem>().Setup(q, _knownRecipies[q]);
            curEntries.Add(newRecipeEntry);
        }
    }

    public void SwitchSubItem(UIUpcyclerRecipeEntryItem sub)
    {
        if(currentOpenItem == sub)
        {
            return;
        }

        if(currentOpenItem == null)
        {
            currentOpenItem = sub;
        }
        else
        {
            currentOpenItem.ToggleSubs();
            currentOpenItem = sub;
        }
    }
    private void ClearEntries()
    {
        if(curEntries.Count <= 0)
        {
            return;
        }

        foreach(GameObject o in curEntries)
        {
            Destroy(o);
        }
        curEntries.Clear();
    }
}
