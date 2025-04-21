using UnityEngine;
using Assets.Scripts.Data;
using System.Collections.Generic;
using Data;
using System.Linq;

public class UIUpcyclerController : UIReUpCyclerSubMenu
{
    public static UIUpcyclerController Instance;

    [Header("MenuParent")]
    [SerializeField] private GameObject upcyclerMenu;

    [Header("ItemSlots")]
    [SerializeField] private List<UIUpcyclerInputSlot> matSlots;

    [Header("PreviewQuest")]
    [SerializeField] private GameObject curQuestItemPreview;
    [SerializeField] private GameObject previewQuestItemPrefab;
    [SerializeField] private Transform parentQuestPreview;
    [SerializeField] private GameObject outputError;
    [SerializeField] private GameObject inputMessage;

    [Header("Buttons")]
    [SerializeField] private GameObject upcycleBtn;
    [SerializeField] private GameObject collectBtn;

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

    private void OnEnable()
    {
        UpcycleManager.OnQuestItemChanged += UpdateQuestItemOutput;
    }

    private void OnDisable()
    {
        UpcycleManager.OnQuestItemChanged -= UpdateQuestItemOutput;
    }

    public override void CloseMenu()
    {
        RemoveAllMaterials();
        Collect();
        upcyclerMenu.SetActive(false);
    }

    public override void OpenMenu()
    {
        upcyclerMenu.SetActive(true);
        //quest recipies needs to be opened here as soon as its implemented.
        UIInventoryController.Instance.OpenSinglePageInventory();
        CheckIfInputMissing();
        EnableUpcycle();
    }

    private void CheckIfInputMissing()
    {
        if (outputError.activeInHierarchy)
        {
            outputError.SetActive(false);
        }

        if(curQuestItemPreview == null && UpcycleManager.Instance.Ingredients.Count <= 0)
        {
            inputMessage.SetActive(true);
            return;
        }
        inputMessage.SetActive(false);
    }

    public void AddMaterial(TrashMaterialData mat)
    {
        if (curQuestItemPreview != null)
        {
            Debug.Log("There's an item in the Output slot!");
        }

        UIUpcyclerInputSlot freeSlot = ReturnFirstFreeSlot();
        if (freeSlot != null && mat != null)
        {
            UpcycleManager.Instance.AddIngredient(mat);
            CheckIfInputMissing();
            freeSlot.Setup(mat);
        }
    }

    public void RemoveMaterial(UIUpcyclerInputSlot slot)
    {
        TrashMaterialData data = slot.CurrentMaterial;

        if (slot != null)
        {
            UpcycleManager.Instance.RemoveIngredient(data);
            slot.ResetSlot();
            CheckIfInputMissing();
        }
    }

    public UIUpcyclerInputSlot ReturnFirstFreeSlot()
    {
        return matSlots.Where(i => i.CurrentMaterial == null).FirstOrDefault();
    }

    private void UpdateQuestItemOutput(QuestItemInstance questItem)
    {
        if(questItem != null)
        {
            GameObject newQuestItem = Instantiate(previewQuestItemPrefab, parentQuestPreview);
            newQuestItem.GetComponent<UIUpcycleQuestItem>().Setup(questItem.ItemData.sprite);
            curQuestItemPreview = newQuestItem;
            return;
        }
        ClearQuestItemPreview();
    }

    public void Upcycle()
    {
        if (!InventoryManager.Instance.HasSpaceForItem())
        {
            Debug.Log("brother inventory is full");
            return;
        }

        if(!UpcycleManager.Instance.Upcycle())
        {
            outputError.SetActive(true);
            return;
        }
        outputError.SetActive(false);
        ClearAllMaterialSlots();
        EnableCollect();
    }

    private void EnableCollect()
    {
        upcycleBtn.SetActive(false);
        collectBtn.SetActive(true);
    }

    private void EnableUpcycle()
    {
        upcycleBtn.SetActive(true);
        collectBtn.SetActive(false);
    }

    public void Collect()
    {
        if(curQuestItemPreview != null)
        {
            UpcycleManager.Instance.CollectQuestItem();
            ClearQuestItemPreview();
            EnableUpcycle();
            CheckIfInputMissing();
        }
    }

    public void RemoveAllMaterials()
    {
        UpcycleManager.Instance.RemoveAllIngredients();
        ClearAllMaterialSlots();
    }

    private void ClearAllMaterialSlots()
    {
        foreach (UIUpcyclerInputSlot slot in matSlots)
        {
            slot.ResetSlot();
        }
        CheckIfInputMissing();
    }

    private void ClearQuestItemPreview()
    {
        Destroy(curQuestItemPreview);
        curQuestItemPreview = null;
    }
}
