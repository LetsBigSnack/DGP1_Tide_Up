using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryUpgradeData
{
    // I supose these will later on be taken from a json with all the upgrades in them?
    // But having an extra class would then still be a good idea right?

    public int slotsToAdd = 3;

    [System.Serializable]
    public class UpgradeCost
    {
        public TrashMaterialData material;
        public int amount;
    }

    public UpgradeCost[] costs;
}

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private InventoryUpgradeData upgradeData;
    public static UpgradeManager Instance;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpgradeInventory()
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif
        if (!CanUpgrade())
        {
            return;
        }

        PayUpgradeCost();
        InventoryManager.Instance.IncreaseMaxItems(upgradeData.slotsToAdd);
        Debug.Log($"Inventory upgraded by {upgradeData.slotsToAdd} slots!");

    }

    private bool CanUpgrade()
    {
        foreach (var cost in upgradeData.costs)
        {
            int playerAmount = InventoryManager.Instance.GetMaterialAmount(cost.material);
            if (playerAmount < cost.amount)
            {
                int missing = cost.amount - playerAmount;
                Debug.Log("Not enough materials to upgrade your inventory");
                Debug.Log("You are missing: " + missing + " " + cost.material);
                return false;
            }
        }

        return true;
    }
    private void PayUpgradeCost()
    {
        foreach (var cost in upgradeData.costs)
        {
            InventoryManager.Instance.RemoveMaterial(cost.material, cost.amount);
        }
    }
}
