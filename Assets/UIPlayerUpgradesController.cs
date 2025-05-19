using Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerUpgradesController : UIShopSubMenu
{
    public static UIPlayerUpgradesController Instance;

    [Header("SubMenu")]
    [SerializeField] private GameObject subMenu;

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
        if(Instance == null)
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
        SwitchUpgrade(UpgradeManager.Instance.ReturnUpgradeByType(UpgradeType.Inventory)[0]);
        subText.text = "Now featuring 300 % more �where did I put that ?� space.";
    }

    public override void CloseMenu()
    {
        ClearMenu();
        subMenu.SetActive(false);
    }

    public void SwitchUpgrade(Upgrade upgrade)
    {
        if(currentUpgrade == upgrade)
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

        List<Upgrade> inventoryUpgrades = UpgradeManager.Instance.ReturnUpgradeByType(UpgradeType.Inventory);

        foreach (Upgrade upgrade in inventoryUpgrades)
        {
            GameObject newLevel = Instantiate(levelPrefab, levelParent);
            newLevel.GetComponent<UIShopLevelItem>().Setup(upgrade);
            _currentLevels.Add(newLevel);
        }
    }

    private void AddRequirements()
    {
        if(_currentRequirements.Count > 0)
        {
            _currentRequirements.Clear();
        }

        if(_currentRequirements == null)
        {
            _currentRequirements = new();
        }

        foreach(UpgradeCost cost in currentUpgrade.costs)
        {
            GameObject newRequirement = Instantiate(reqItemPrefab, reqItemParent);
            newRequirement.GetComponent<UIShopRequirementItem>().Setup(cost,currentUpgrade.CostIsAvailable(cost));
            _currentRequirements.Add(newRequirement);
        }
        GameObject itemToRefresh = _currentLevels.Find(t => t.GetComponent<UIShopLevelItem>().GetCurrentUpgrade() == currentUpgrade);
        UIShopLevelItem uiShopLevelItem = itemToRefresh.GetComponent<UIShopLevelItem>();
        uiShopLevelItem.Setup(currentUpgrade);
        uiShopLevelItem.SetSelected();
    }

    private void ClearMenu()
    {
        foreach(GameObject o in _currentLevels)
        {
            Destroy(o);
        }
        _currentLevels.Clear();

        foreach(GameObject o in _currentRequirements)
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

        foreach (UpgradeCost cost in currentUpgrade.costs)
        {
            GameObject costItem = _currentRequirements.Find(r => r.GetComponent<UIShopRequirementItem>().GetCurrentCost() == cost);
            costItem.GetComponent<UIShopRequirementItem>().Setup(cost, currentUpgrade.CostIsAvailable(cost));
        }
    }

    public void Upgrade()
    {
        if (!currentUpgrade.isUnlocked)
        {
            Upgrade lastCurrUpgrade = currentUpgrade;
            currentUpgrade.ApplyUpgrade();
            RefreshCurrentUpgrade();
            if(lastCurrUpgrade.nextUpgrade == null)
            {
                return;
            }
            currentUpgrade = lastCurrUpgrade.nextUpgrade;
            SwitchUpgrade(currentUpgrade);
        }
    }

}
