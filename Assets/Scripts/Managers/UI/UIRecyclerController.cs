using UnityEngine;
using Assets.Scripts.Data;
using System.Collections.Generic;
using Data;
using System.Linq;

public class UIRecyclerController : UIReUpCyclerSubMenu
{
    public static UIRecyclerController Instance;

    [Header("MenuParent")]
    [SerializeField] private GameObject recyclerMenu;

    [Header("ItemSlots")]
    [SerializeField] private List<UIRecyclerInputSlot> itemSpaces;

    [Header("PreviewMaterials")]
    [SerializeField] private List<GameObject> curPreviewMaterials;
    [SerializeField] private GameObject previewMaterialPrefab;
    [SerializeField] private Transform previewMaterialParent;

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

    private void OnEnable()
    {
        RecyclerManager.OnStoredItemsChanged += UpdateMaterialsToPreview;
    }

    private void OnDisable()
    {
        RecyclerManager.OnStoredItemsChanged -= UpdateMaterialsToPreview;
    }

    public override void CloseMenu()
    {
        RemoveAllItems();
        recyclerMenu.SetActive(false);
    }

    public override void OpenMenu()
    {
        recyclerMenu.SetActive(true);
        if (UIJournalManager.Instance.GetCurrentState() != JournalType.Inventory && UIJournalManager.Instance.GetCurrentState() != JournalType.Recipies)
        {
            UIJournalManager.Instance.SwitchState(JournalType.Inventory);
        }
    }

    public void AddItem(ItemInstance item)
    {
        UIRecyclerInputSlot freeSlot = ReturnFirstFreeSlot();
        if(freeSlot != null && item != null)
        {
            RecyclerManager.Instance.AddItemToRecycler(item);
            freeSlot.Setup(item);
        }
    }

    public void RemoveItem(UIRecyclerInputSlot slot)
    {
        ItemInstance item = slot.CurrentItem;

        if (slot != null){
            RecyclerManager.Instance.RemoveItemFromRecycler(item);
            slot.ResetSlot();
        }
    }

    public UIRecyclerInputSlot ReturnFirstFreeSlot()
    {
        return itemSpaces.Where(i => i.Data == null).FirstOrDefault();
    }


    private void UpdateMaterialsToPreview(ItemInstance item, bool isRemoved)
    {
        List<TrashMaterialData> materials = item.ItemData.Materials;

        if (materials == null || item == null)
        {
            return;
        }

        foreach(TrashMaterialData mat in materials)
        {
            CheckMaterialPreview(mat, isRemoved);
        }
    }

    private void CheckMaterialPreview(TrashMaterialData mat, bool isRemoved)
    {
        if (IsMaterialInPreview(mat.type))
        {
            GameObject currentPreviewObj = curPreviewMaterials.Where(m => m.GetComponent<UIRecycleMaterialItem>().Type == mat.type).FirstOrDefault();
            UIRecycleMaterialItem curPrevMat = currentPreviewObj.GetComponent<UIRecycleMaterialItem>();
            if (currentPreviewObj != null)
            {
                if (isRemoved)
                {
                    if (curPrevMat.CurrentAmount - 1 <= 0)
                    {
                        Destroy(currentPreviewObj);
                        curPreviewMaterials.Remove(currentPreviewObj);
                        return;
                    }
                    curPrevMat.ChangeAmount(-1);
                    return;
                }
            }
            curPrevMat.ChangeAmount(1);
            return;
        }

        if (!isRemoved)
        {
            GameObject newPreview = Instantiate(previewMaterialPrefab, previewMaterialParent);
            newPreview.GetComponent<UIRecycleMaterialItem>().Setup(mat.type, mat.sprite, 1);
            curPreviewMaterials.Add(newPreview);
        }
    }

    public void Recycle()
    {
        RecyclerManager.Instance.RecycleItems();
        ClearAllItemSlots();
        ClearMaterialPreview();
    }

    public void RemoveAllItems()
    {
        RecyclerManager.Instance.RemoveAllItems();
        ClearAllItemSlots();
        ClearMaterialPreview();
    }

    private void ClearAllItemSlots()
    {
        foreach(UIRecyclerInputSlot slot in itemSpaces)
        {
            slot.ResetSlot();
        }
    }

    private bool IsMaterialInPreview(TrashMaterialType type)
    {
        return curPreviewMaterials.Any(m => m.GetComponent<UIRecycleMaterialItem>().Type == type);
    }

    private void ClearMaterialPreview()
    {
        foreach(GameObject obj in curPreviewMaterials)
        {
            Destroy(obj);
        }
        curPreviewMaterials.Clear();
    }
}
