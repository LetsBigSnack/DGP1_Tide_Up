using Data;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBoatUpgradeController : UIShopSubMenu
{
    public static UIBoatUpgradeController Instance;

    [Header("SubMenu")]
    [SerializeField] private GameObject subMenu;
    [SerializeField] private UpgradeType currentType = UpgradeType.BoatSpeed;

    [Header("Current Upgrade Selected")]
    [SerializeField] private Upgrade currentUpgrade;
    private List<GameObject> _currentLevels = new List<GameObject>();
    private List<GameObject> _currentRequirements = new List<GameObject>();

    [Header("Upgrade Info")]
    [SerializeField] private Image upgradeImage;
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [SerializeField] private TextMeshProUGUI subText;
    [SerializeField] private Image buttonImage;

    [Header("Levels")]
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Transform levelParent;

    [Header("Required Items")]
    [SerializeField] private GameObject reqItemPrefab;
    [SerializeField] private Transform reqItemParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OpenMenu()
    {
        subMenu.SetActive(true);
        SwitchUpgrade(UpgradeManager.Instance.ReturnUpgradeByType(UpgradeType.BoatSpeed)[0]);
    }

    public override void CloseMenu()
    {
        ClearMenu();
        subMenu.SetActive(false);
    }

    public void SwitchTypeByInt(int i)
    {
        SwitchType((UpgradeType)i);
    }

    private void SwitchType(UpgradeType type)
    {
        if(currentType == type)
        {
            return;
        }

        currentType = type;

        switch (type)
        {
            case UpgradeType.BoatSpeed:
                SwitchUpgrade(UpgradeManager.Instance.ReturnUpgradeByType(UpgradeType.BoatSpeed)[0]);
                subText.text = "Less chug, more zoom! Just don�t race the dolphins� again.";
                break;
            case UpgradeType.OilClean:
                SwitchUpgrade(UpgradeManager.Instance.ReturnUpgradeByType(UpgradeType.OilClean)[0]);
                subText.text = "Suck it up, buttercup!";
                break;
            case UpgradeType.WaterAccess:
                SwitchUpgrade(UpgradeManager.Instance.ReturnUpgradeByType(UpgradeType.WaterAccess)[0]);
                subText.text = "With great depth comes great adventure...";
                break;
        }
    }

    public void SwitchUpgrade(Upgrade upgrade)
    {
        if (currentUpgrade == upgrade)
        {
            return;
        }

        ClearMenu();
        currentUpgrade = upgrade;
        //upgradeImage.sprite = upgrade.sprite;
        upgradeDescription.text = upgrade.description;
        if (!upgrade.CanUpgrade() || upgrade.isUnlocked)
        {
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0.5f);
        }
        else
        {
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 1f);
        }
        AddLevels();
        AddRequirements();
    }

    public void SwitchBetweenUpgrades(Upgrade upgrade)
    {
        foreach (GameObject o in _currentRequirements)
        {
            Destroy(o);
        }
        _currentRequirements.Clear();

        AddRequirements();

        currentUpgrade = upgrade;
        //upgradeImage.sprite = upgrade.sprite;
        upgradeDescription.text = upgrade.description;
        if (!upgrade.CanUpgrade() || upgrade.isUnlocked)
        {
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0.5f);
        }
        else
        {
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 1f);
        }
    }

    private void AddLevels()
    {
        if (_currentLevels.Count > 0)
        {
            _currentLevels.Clear();
        }

        if (_currentLevels == null)
        {
            _currentLevels = new();
        }

        List<Upgrade> inventoryUpgrades = UpgradeManager.Instance.ReturnUpgradeByType(currentType);

        foreach (Upgrade upgrade in inventoryUpgrades)
        {
            GameObject newLevel = Instantiate(levelPrefab, levelParent);
            newLevel.GetComponent<UIShopLevelItem>().Setup(upgrade);
            _currentLevels.Add(newLevel);
            if(_currentLevels.Count == 1)
            {
                UIEventSystemHelper.Instance.SetFirstSelectedItem(newLevel);
            }
        }

        GameObject itemToRefresh = _currentLevels.Find(t => t.GetComponent<UIShopLevelItem>().GetCurrentUpgrade() == currentUpgrade);
        UIShopLevelItem uiShopLevelItem = itemToRefresh.GetComponent<UIShopLevelItem>();
        uiShopLevelItem.Setup(currentUpgrade);
        uiShopLevelItem.SetSelected();
    }

    private void AddRequirements()
    {
        if (_currentRequirements.Count > 0)
        {
            _currentRequirements.Clear();
        }

        if (_currentRequirements == null)
        {
            _currentRequirements = new();
        }

        foreach (UpgradeCost cost in currentUpgrade.costs)
        {
            GameObject newRequirement = Instantiate(reqItemPrefab, reqItemParent);
            newRequirement.GetComponent<UIShopRequirementItem>().Setup(cost, currentUpgrade.CostIsAvailable(cost));
            _currentRequirements.Add(newRequirement);
        }
    }

    private void ClearMenu()
    {
        foreach (GameObject o in _currentLevels)
        {
            Destroy(o);
        }
        _currentLevels.Clear();

        foreach (GameObject o in _currentRequirements)
        {
            Destroy(o);
        }
        _currentRequirements.Clear();

        currentUpgrade = null;
    }

    private void RefreshCurrentUpgrade()
    {
        GameObject itemToRefresh = _currentLevels.Find(t => t.GetComponent<UIShopLevelItem>().GetCurrentUpgrade() == currentUpgrade);
        itemToRefresh.GetComponent<UIShopLevelItem>().Setup(currentUpgrade);

        foreach(UpgradeCost cost in currentUpgrade.costs)
        {
            GameObject costItem = _currentRequirements.Find(r => r.GetComponent<UIShopRequirementItem>().GetCurrentCost() == cost);
            costItem.GetComponent<UIShopRequirementItem>().Setup(cost, currentUpgrade.CostIsAvailable(cost));
        }
        
    }

    public void Upgrade()
    {
        if (!currentUpgrade.isUnlocked)
        {
            currentUpgrade.ApplyUpgrade();
            if (!currentUpgrade.isUnlocked)
            {
                return;
            }
            Upgrade lastCurrUpgrade = currentUpgrade;
            RefreshCurrentUpgrade();
            if (lastCurrUpgrade.nextUpgrade == null)
            {
                return;
            }
            currentUpgrade = lastCurrUpgrade.nextUpgrade;
            SwitchUpgrade(currentUpgrade);
        }
    }

}
