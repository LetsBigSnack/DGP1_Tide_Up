using UnityEngine;
using UnityEngine.UI;
using Data;
using System;

public class UIInventoryItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private ItemInstance item;
    [SerializeField] private GameObject borderIcon;

    public void Setup(ItemInstance data)
    {
        item = data;
        image.sprite = data.ItemData.sprite;
    }

    public void OnClick()
    {
        if (UITideUpBoxManager.Instance.IsOpen)
        {
            return;
        }

        JournalType curJournalState = UIJournalManager.Instance.GetCurrentState();
        ShopType curShopState = UIShopManager.Instance.GetCurrentState();
        ReUpcyclerType curReUpcyclerState = UIReUpcycleManager.Instance.GetCurrentState();

        if (curJournalState == JournalType.Inventory
            && curReUpcyclerState == ReUpcyclerType.Recycler
            && curShopState == ShopType.Closed)
        {
            UIRecyclerController.Instance.AddItem(item);
        }

    public void OnHover()
    {
        if(UIJournalManager.Instance.GetCurrentState() == JournalType.Inventory && UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed && !UITideUpBoxManager.Instance.IsOpen)
        {
            UIItemDetailsHelper.Instance.SetupDescription(item.ItemData.title, item.ItemData.description, item.ItemData.sprite, item.ItemData.Materials);
            UIItemDetailsHelper.Instance.SetGameObjectAsSelected(this);
        }
    }

    public void ToggleIcon()
    {
        borderIcon.SetActive(!borderIcon.activeInHierarchy);
        if (UIJournalManager.Instance.GetCurrentState() == JournalType.Inventory && UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed && !UITideUpBoxManager.Instance.IsOpen)
        {
            UIItemDetailsHelper.Instance.ResetDescription();
        }
    }
}
