using ScriptableObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeEntryItem : MonoBehaviour
{
    private QuestItemData _questItem;
    private List<RecipeData> _knownRecipies;

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;

    [SerializeField] private GameObject selectCircle;

    public void Setup(QuestItemData questItem, List<RecipeData> knownRecipies)
    {
        if(questItem == null || knownRecipies == null)
        {
            return;
        }

        this._questItem = questItem;
        this._knownRecipies = knownRecipies;

        this.image.sprite = questItem.sprite;
        this.nameText.text = questItem.title;
        this.amountText.text = "Found recipies: " + knownRecipies.Count.ToString();
    }

    public void OnClick()
    {
        if (_questItem == null || _knownRecipies == null)
        {
            return;
        }

        if(UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed)
        {
            UIRecipeDescriptionHelper.Instance.Setup(_questItem, _knownRecipies);
            UIRecipeDescriptionHelper.Instance.SetGameObjectAsSelected(this);
        }

        SoundManager.Instance.PlaySFX("Click");
    }

    public void OnSelect()
    {
        UIRecipeDescriptionHelper.Instance.SetGameObjectAsSelected(this);

        SoundManager.Instance.PlaySFX("Click");
    }

    public void OnSubmit()
    {
        if (_questItem == null || _knownRecipies == null)
        {
            return;
        }

        if (UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed)
        {
            UIRecipeDescriptionHelper.Instance.Setup(_questItem, _knownRecipies);
            UIRecipeDescriptionHelper.Instance.SetGameObjectAsSelected(this);
        }
    }

    public void ToggleIcon()
    {
        selectCircle.SetActive(!selectCircle.activeInHierarchy);
    }
}
