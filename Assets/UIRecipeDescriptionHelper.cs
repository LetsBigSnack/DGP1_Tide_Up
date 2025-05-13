using ScriptableObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeDescriptionHelper : MonoBehaviour
{
    public static UIRecipeDescriptionHelper Instance;

    private QuestItemData _curQuestItem;
    private List<RecipeData> _curRecipies = new List<RecipeData>();
    private List<GameObject> _curDisplayedMaterials = new List<GameObject>();
    private List<GameObject> _curDisplayedIndicators = new List<GameObject>();

    [Header("Item Description")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Material Carousel")]
    [SerializeField] private int currentPosition;
    [SerializeField] private GameObject materialIconPrefab;
    [SerializeField] private Transform materialIconParent;

    [SerializeField] private Transform indicatorParent;
    [SerializeField] private GameObject indicatorFull;
    [SerializeField] private GameObject indicatorEmpty;

    public UIRecipeEntryItem _currentSelectedItem;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Setup(QuestItemData questItem, List<RecipeData> knownRecipies)
    {
        if(questItem == null || knownRecipies == null)
        {
            return;
        }

        this.itemImage.sprite = questItem.sprite;
        this.nameText.text = questItem.title;
        this.description.text = questItem.description;

        this._curQuestItem = questItem;
        this._curRecipies = knownRecipies;

        currentPosition = 0;
        FillMaterialParent();
        UpdateIndicators();
    }

    public void PreviousRecipe()
    {
        if(currentPosition == 0)
        {
            currentPosition = _curRecipies.Count-1;
        }
        else
        {
            currentPosition--;
        }
        FillMaterialParent();
        UpdateIndicators();
    }

    public void NextRecipe()
    {
        if (currentPosition +1 == _curRecipies.Count)
        {
            currentPosition = 0;
        }
        else
        {
            currentPosition++;
        }
        FillMaterialParent();
        UpdateIndicators();
    }

    private void UpdateIndicators()
    {
        ClearIndicators();

        for(int i = 0; i < _curRecipies.Count; i++)
        {
            GameObject indicator;

            if(i == currentPosition)
            {
                indicator = Instantiate(indicatorFull, indicatorParent);
            }
            else
            {
                indicator = Instantiate(indicatorEmpty, indicatorParent);
            }

            _curDisplayedIndicators.Add(indicator);
        }
    }

    private void ClearIndicators()
    {
        if(_curDisplayedIndicators.Count <= 0)
        {
            return;
        }

        foreach (GameObject o in _curDisplayedIndicators)
        {
            Destroy(o);
        }

        _curDisplayedIndicators.Clear();
    }

    private void FillMaterialParent()
    {
        ClearMaterialParent();
        foreach(TrashMaterialData t in _curRecipies[currentPosition].ingredients)
        {
            GameObject trashMaterial = Instantiate(materialIconPrefab, materialIconParent);
            trashMaterial.transform.GetChild(0).GetComponent<Image>().sprite = t.sprite;
            _curDisplayedMaterials.Add(trashMaterial);
        }
    }

    private void ClearMaterialParent()
    {
        if(_curDisplayedMaterials.Count <= 0)
        {
            return;
        }

        foreach(GameObject o in _curDisplayedMaterials)
        {
            Destroy(o);
        }

        _curDisplayedMaterials.Clear();
    }

    public void SetGameObjectAsSelected(UIRecipeEntryItem item)
    {
        if (_currentSelectedItem == item)
        {
            return;
        }

        if (_currentSelectedItem == null)
        {
            _currentSelectedItem = item;
            item.ToggleIcon();
            return;
        }

        _currentSelectedItem.ToggleIcon();
        _currentSelectedItem = item;
        _currentSelectedItem.ToggleIcon();
    }
}
