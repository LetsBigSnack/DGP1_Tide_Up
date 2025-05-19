using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/OilCleanUpgrade")]
public class OilCleanUpgrade : Upgrade
{
    public int cleaningRange;

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
        SoundManager.Instance.PlaySFX("Buy");
        Debug.Log($"You extended your cleaning range by " + cleaningRange + "m !");
    }
}