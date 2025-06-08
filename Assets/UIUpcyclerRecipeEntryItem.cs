using ScriptableObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpcyclerRecipeEntryItem : MonoBehaviour
{
    private QuestItemData _questItem;
    private List<RecipeData> _knownRecipies = new List<RecipeData>();
    private List<GameObject> _curSubItems = new List<GameObject>();

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image buttonImage;

    [SerializeField] private Sprite closedArrow;
    [SerializeField] private Sprite openArrow;

    [SerializeField] private GameObject subRecipePrefab;
    [SerializeField] private Transform subRecipeParent;

    [SerializeField] private GameObject selectCircle;

    public void Setup(QuestItemData questItem, List<RecipeData> knownRecipies)
    {
        if (questItem == null || knownRecipies == null)
        {
            return;
        }

        this._questItem = questItem;
        this._knownRecipies = knownRecipies;

        this.image.sprite = questItem.sprite;
        this.nameText.text = questItem.title;
        this.amountText.text = "Found recipies: " + knownRecipies.Count.ToString();
        selectCircle.SetActive(false);
    }

    public void OnClick()
    {
        if (_questItem == null || _knownRecipies == null)
        {
            return;
        }
        UIRecipeController.Instance.SwitchSubItem(this);
    }

    public void ToggleSubs()
    {
        if (!IsRecipeOpen())
        {
            CreateSubs();
            buttonImage.sprite = openArrow;
        }
        else
        {
            ClearSubs();
            buttonImage.sprite = closedArrow;
        }
    }

    public bool IsRecipeOpen()
    {
        return _curSubItems.Count > 0;
    }
    private void CreateSubs()
    {
        foreach(RecipeData r in _knownRecipies)
        {
            GameObject sub = Instantiate(subRecipePrefab, subRecipeParent);
            sub.GetComponent<UIUpcyclerSubRecipeEntryItem>().Setup(r.ingredients);
            _curSubItems.Add(sub);
        }
    }

    public void OnSelect()
    {
        selectCircle.SetActive(true);
    }

    public void OnDeselect()
    {
        selectCircle.SetActive(false);
    }

    private void ClearSubs()
    {
        if(_curSubItems.Count <= 0)
        {
            return;
        }

        foreach(GameObject o in _curSubItems)
        {
            Destroy(o);
        }

        _curSubItems.Clear();
    }
}
