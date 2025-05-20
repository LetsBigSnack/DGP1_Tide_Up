using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    BoatSpeed,
    Inventory,
    OilClean,
    WaterAccess
}

[Serializable]
public class UpgradeCost
{
    public bool isQuestItem;
    public TrashMaterialData material;
    public QuestItemData itemData;
    public int amount;
}

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public abstract class Upgrade : ScriptableObject
{
    [Header("Setup")]
    public UpgradeType type;
    public int upgradeLevel;
    public Sprite sprite;
    public string description;

    [Header("Upgrade Costs")]
    public List<UpgradeCost> costs = new List<UpgradeCost>();

    [Header("Upgrade State")]
    public Upgrade previousUpgrade;
    public Upgrade nextUpgrade;
    public bool isUnlocked = false;

    public abstract void ApplyUpgrade();

    public bool CanUpgrade()
    {
        if (!IsPreviousUpgradeUnlocked())
        {
            return false;
        }
        
        
        bool canBuy = true;
        foreach (UpgradeCost cost in costs)
        {
            if (!cost.isQuestItem)
            {
                int playerAmount = InventoryManager.Instance.GetMaterialAmount(cost.material.type);
                if (playerAmount < cost.amount)
                {
                    int missing = cost.amount - playerAmount;
                    Debug.Log("Not enough materials to upgrade your inventory");
                    Debug.Log("You are missing: " + missing + " " + cost.material);
                    canBuy = false;
                }
            }
            else
            {
                ItemInstance item = new ItemInstance(cost.itemData);
                if (!InventoryManager.Instance.IsItemInInventory(item, cost.amount))
                {
                    canBuy = false;
                }
            }
        }

        return canBuy;
    }

    public bool CostIsAvailable(UpgradeCost cost)
    {
        bool costIsAvailable = false;

            if (!cost.isQuestItem)
            {
                int playerAmount = InventoryManager.Instance.GetMaterialAmount(cost.material.type);
                if (playerAmount < cost.amount)
                {
                    costIsAvailable = false;
                }
                else
                {
                    costIsAvailable = true;
                }
            }
            else
            {
                ItemInstance item = new ItemInstance(cost.itemData);
                if (!InventoryManager.Instance.IsItemInInventory(item, cost.amount))
                {
                    costIsAvailable = false;
                }
                else
                {
                    costIsAvailable = true;
                }
            }
        return costIsAvailable;
    }

    public void PayUpgradeCost()
    {
        foreach (UpgradeCost cost in costs)
        {
            if (!cost.isQuestItem)
            {
                InventoryManager.Instance.RemoveMaterial(cost.material.type, cost.amount);
            }
            else
            {
                ItemInstance item = new ItemInstance(cost.itemData);
                for (int i = 0; i < cost.amount; i++)
                {
                    InventoryManager.Instance.RemoveItem(item);
                }
            }
        }

        //TODO: Add sound
        UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "You upgraded your " + type + " to level: " + upgradeLevel);

    }

    public bool IsPreviousUpgradeUnlocked()
    {
        if(previousUpgrade == null || previousUpgrade.isUnlocked)
        {
            return true;
        }
        return false;
    } 
}



