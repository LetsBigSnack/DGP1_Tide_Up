using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/BoatSpeedUpgrade")]
public class BoatSpeedUpgrade : Upgrade
{
    public float boatSpeed;

    public override void ApplyUpgrade()
    {
        if (!CanUpgrade() || !IsPreviousUpgradeUnlocked())
        {
            Debug.Log("Cant Upgrade");
            return;
        }

        PayUpgradeCost();
        //TODO ADD FUNCTIONALITY!
        isUnlocked = true;
        Debug.Log($"Boat speed upgraded by " + boatSpeed + " !");
    }
}