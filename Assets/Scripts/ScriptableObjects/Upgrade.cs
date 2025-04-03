using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public abstract class Upgrade : ScriptableObject
{
    public List<UpgradeCost> costs = new();

    public abstract void ApplyUpgrade();

    public bool CanUpgrade()
    {
        foreach (UpgradeCost cost in costs)
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
    public void PayUpgradeCost()
    {
        foreach (UpgradeCost cost in costs)
        {
            InventoryManager.Instance.RemoveMaterial(cost.material, cost.amount);
        }
    }
}
