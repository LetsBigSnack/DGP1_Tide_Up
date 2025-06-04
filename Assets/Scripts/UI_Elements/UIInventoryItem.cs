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
    
    public bool IsEmpty()
    {
        return isEmpty;
    }

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

            if (borderIcon.activeInHierarchy)
            {
                borderIcon.SetActive(false);
            }
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
            UIInventoryHelper.Instance.SetGameObjectAsSelected(this);
        }

        if (curJournalState == JournalType.Inventory && curReUpcyclerState == ReUpcyclerType.Upcycler && curShopState == ShopType.Closed)
        {
            //TODO: Add error sound
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Try doing that in the Recycler in the other tab");
        }

        if (curJournalState == JournalType.Inventory &&
            curReUpcyclerState == ReUpcyclerType.Closed &&
            !UITideUpBoxManager.Instance.IsOpen &&
            curShopState == ShopType.Closed)
        {
            UIItemDetailsHelper.Instance.SetupDescription(item.ItemData.title, item.ItemData.description, item.ItemData.sprite, item.GetMaterials());
            UIInventoryHelper.Instance.SetGameObjectAsSelected(this);
        }
    }

    public void OnSelect()
    {
        if (isEmpty || !InputDeviceHelper.Instance.IsController())
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
            UIInventoryHelper.Instance.SetGameObjectAsSelected(this);
        }

        if (curJournalState == JournalType.Inventory
            && curReUpcyclerState != ReUpcyclerType.Closed
            && curShopState == ShopType.Closed)
        {
            UIInventoryHelper.Instance.SetGameObjectAsSelected(this);
        }
    }

    public void OnHover()
    {
        if (InputDeviceHelper.Instance.IsController())
        {
            return;
        }

        JournalType curJournalState = UIJournalManager.Instance.GetCurrentState();
        ShopType curShopState = UIShopManager.Instance.GetCurrentState();
        ReUpcyclerType curReUpcyclerState = UIReUpcycleManager.Instance.GetCurrentState();

        if (curJournalState == JournalType.Inventory
        && curReUpcyclerState != ReUpcyclerType.Closed
        && curShopState == ShopType.Closed)
        {
            UIInventoryHelper.Instance.SetGameObjectAsSelected(this);
        }
    }

    public void OffHover()
    {
        if (InputDeviceHelper.Instance.IsController())
        {
            return;
        }

        UIInventoryHelper.Instance.SetGameObjectAsSelected(null);
    }

    public void OnDeselect()
    {
        if (!InputDeviceHelper.Instance.IsController())
        {
            return;
        }

        if (isEmpty)
        {
            if (borderIcon.activeInHierarchy)
            {
                borderIcon.SetActive(false);
            }
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
