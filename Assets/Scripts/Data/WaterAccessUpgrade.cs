using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/WaterAccessUpgrade")]
public class WaterAccessUpgrade : Upgrade
{
    public int waterLevel;

    public override void ApplyUpgrade()
    {
        if (!CanUpgrade())
        {
            Debug.Log("Cant Upgrade");
            return;
        }

        PayUpgradeCost();
        //TODO ADD FUNCTIONALITY!
        isUnlocked = true;
        Debug.Log($"You can now travel waters of level " + waterLevel + " !");
    }
}