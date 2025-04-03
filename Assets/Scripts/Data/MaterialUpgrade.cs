using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/MaterialUpgrade")]
public class MaterialUpgrade : Upgrade
{
    public int slotsToAdd = 10;

    public override void ApplyUpgrade()
    {
        PayUpgradeCost();
        InventoryManager.Instance.IncreaseMaxMaterials(slotsToAdd);
        Debug.Log($"Material inventory upgraded by " + slotsToAdd + " slots!");
    }
}
