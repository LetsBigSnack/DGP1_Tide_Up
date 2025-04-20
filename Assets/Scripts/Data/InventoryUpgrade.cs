using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/InventoryUpgrade")]
public class InventoryUpgrade : Upgrade
{
    public int slotsToAdd = 3;
    
    public override void ApplyUpgrade()
    {
        if (!CanUpgrade())
        {
            Debug.Log("Cant Upgrade");
            return;
        }
        PayUpgradeCost();
        InventoryManager.Instance.IncreaseMaxItems(slotsToAdd);
        isUnlocked = true;
        Debug.Log($"Inventory upgraded by " + slotsToAdd + " slots!");
    }
}
