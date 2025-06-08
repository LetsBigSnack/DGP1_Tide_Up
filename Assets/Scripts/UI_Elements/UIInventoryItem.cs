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
    [SerializeField] private GameObject star;
    
    public bool IsEmpty()
    {
        return isEmpty;
    }

    public void Setup(ItemInstance data = null, bool isEmpty = true)
    {  
        this.isEmpty = isEmpty;
        if (isEmpty)
        {
            star.SetActive(false);
            item = null;
            image.color = new Color(1, 1, 1, 0);

            if (borderIcon.activeInHierarchy)
            {
                borderIcon.SetActive(false);
            }
            return;
        }
        image.sprite = data.ItemData.sprite;
        item = data;
        image.color = new Color(1, 1, 1, 1);

        if(data.ItemQuality == ItemQuality.High)
        {
            star.SetActive(true);
        }
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

        if (curReUpcyclerState == ReUpcyclerType.Upcycler)
        {
            UIReUpcycleManager.Instance.SwitchState(ReUpcyclerType.Recycler);
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Switched to recycler!");
            return;
        }


        if (curJournalState == JournalType.Inventory
            && curReUpcyclerState == ReUpcyclerType.Recycler
            && curShopState == ShopType.Closed)
        {
            UIRecyclerController.Instance.AddItem(item);
            ToggleIcon(false);
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
        if (!InputDeviceHelper.Instance.IsController())
        {
            return;
        }

        if (isEmpty)
        {
            ToggleIcon(true);
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
            ToggleIcon(false);
            return;
        }

        if (UITideUpBoxManager.Instance.IsOpen)
        {
            ToggleIcon(false);
        }
    }

    public void ToggleIcon(bool active)
    {
        borderIcon.SetActive(active);
    }

}
