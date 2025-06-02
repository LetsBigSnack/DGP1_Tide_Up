using UnityEngine;
using UnityEngine.UI;
using Data;
using System;

public class UIInventoryItem : MonoBehaviour
{
    [SerializeField] private bool isEmpty;
    [SerializeField] private Image image;
    [SerializeField] private ItemInstance item;
    [SerializeField] private GameObject borderIcon;
    
    public void Setup(ItemInstance data = null, bool isEmpty = true)
    {
        Button button = GetComponent<Button>();
        Navigation nav = button.navigation;
        
        this.isEmpty = isEmpty;
        if (isEmpty)
        {
            item = null;
            image.color = new Color(1, 1, 1, 0);
            button.interactable = false;
            
            nav.mode = Navigation.Mode.None;
            button.navigation = nav;
            return;
        }
        item = data;
        image.color = new Color(1, 1, 1, 1);
        button.interactable = true;
        nav.mode = Navigation.Mode.Automatic;
        button.navigation = nav;
        image.sprite = data.ItemData.sprite;
    }

    public void OnClick()
    {
        if (isEmpty)
        {
            return;
        }
        
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

        if (curJournalState == JournalType.Inventory && curReUpcyclerState == ReUpcyclerType.Upcycler && curShopState == ShopType.Closed)
        {
            //TODO: Add error sound
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Try doing that in the Recycler in the other tab");
        }
    }

    public void OnSelect()
    {
        if (isEmpty)
        {
            return;
        }
        
        if (UITideUpBoxManager.Instance.IsOpen)
        {
            borderIcon.SetActive(!borderIcon.activeInHierarchy);
            return;
        }

        JournalType curJournalState = UIJournalManager.Instance.GetCurrentState();
        ShopType curShopState = UIShopManager.Instance.GetCurrentState();
        ReUpcyclerType curReUpcyclerState = UIReUpcycleManager.Instance.GetCurrentState();

        if (curJournalState == JournalType.Inventory &&
            curReUpcyclerState == ReUpcyclerType.Closed &&
            !UITideUpBoxManager.Instance.IsOpen &&
            curShopState == ShopType.Closed)
        {
            UIItemDetailsHelper.Instance.SetupDescription(item.ItemData.title, item.ItemData.description, item.ItemData.sprite, item.GetMaterials());
            UIItemDetailsHelper.Instance.SetGameObjectAsSelected(this);
        }
    }

    public void OnDeselect()
    {
        if (isEmpty)
        {
            return;
        }
        
        if (UITideUpBoxManager.Instance.IsOpen)
        {
            borderIcon.SetActive(!borderIcon.activeInHierarchy);
        }
    }


    public void ToggleIcon()
    {
        borderIcon.SetActive(!borderIcon.activeInHierarchy);
    }
}
