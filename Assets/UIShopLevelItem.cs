using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopLevelItem : MonoBehaviour
{
    [SerializeField] private UpgradeType type;
    [SerializeField] private Upgrade currentUpgrade;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Sprite notUnlockedSprite;
    [SerializeField] private Sprite selectedBorder;
    [SerializeField] private Sprite notSelectedBorder;
    [SerializeField] private GameObject upgradeDone;

    public Upgrade GetCurrentUpgrade()
    {
        return currentUpgrade;
    }

    public void Setup(Upgrade upgrade)
    {
        type = upgrade.type;
        currentUpgrade = upgrade;
        levelText.text = "LEVEL " + upgrade.upgradeLevel.ToString();

        if (upgrade.isUnlocked)
        {
            itemImage.sprite = upgrade.sprite;
            upgradeDone.SetActive(true);
        }
        else
        {
            itemImage.sprite = notUnlockedSprite;
            upgradeDone.SetActive(false);
        }

        if (upgrade.previousUpgrade != null)
        {
            if (upgrade.previousUpgrade.isUnlocked)
            {
                itemImage.sprite = upgrade.sprite;
            }
        }
        if(upgrade.upgradeLevel == 1)
        {
            itemImage.sprite = upgrade.sprite;
        }
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

    public void SetSelected()
    {
        if(borderImage.sprite == selectedBorder)
        {
            borderImage.sprite = notSelectedBorder;
            return;
        }
        borderImage.sprite = selectedBorder;
    }
}
