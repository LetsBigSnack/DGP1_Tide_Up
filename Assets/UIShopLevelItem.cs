using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopLevelItem : MonoBehaviour
{
    [SerializeField] private UpgradeType type;
    [SerializeField] private Upgrade currentUpgrade;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Sprite notUnlockedSprite;

    public Upgrade GetCurrentUpgrade()
    {
        return currentUpgrade;
    }

    public void Setup(Upgrade upgrade)
    {
        type = upgrade.type;
        currentUpgrade = upgrade;
        image.sprite = upgrade.isUnlocked? upgrade.sprite : notUnlockedSprite;
        levelText.text = "LEVEL " + upgrade.upgradeLevel.ToString();
    }

    public void OnClick()
    {
        switch (type)
        {
            case UpgradeType.BoatSpeed:
                UIBoatUpgradeController.Instance.SwitchUpgrade(currentUpgrade);
                break;
            case UpgradeType.Inventory:
                UIPlayerUpgradesController.Instance.SwitchUpgrade(currentUpgrade);
                break;
            case UpgradeType.OilClean:
                UIBoatUpgradeController.Instance.SwitchUpgrade(currentUpgrade);
                break;
            case UpgradeType.WaterAccess:
                UIBoatUpgradeController.Instance.SwitchUpgrade(currentUpgrade);
                break;
        }
    }
}
