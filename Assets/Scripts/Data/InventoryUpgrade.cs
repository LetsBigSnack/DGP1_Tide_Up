using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/InventoryUpgrade")]
public class InventoryUpgrade : Upgrade
{
    public int slotsToAdd = 3;

    public override void ApplyUpgrade()
    {
        InventoryManager.Instance.IncreaseMaxItems(slotsToAdd);
        Debug.Log($"Inventory upgraded by " + slotsToAdd + " slots!");
    }
}
