using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<Upgrade> upgrades;
    
    public static UpgradeManager Instance;

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

    
    //TODO: list all the upgrade in the UI Later
    public List<Upgrade> GetUpgrades()
    {
        return upgrades;
    }

    public List<Upgrade> ReturnUpgradeByType(UpgradeType type)
    {
        return upgrades.Where(u => u.type == type).OrderBy(x => x.upgradeLevel).ToList();
    }

    public Upgrade GetUpgrade(int upgradeIndex)
    {
        return upgrades[(int)upgradeIndex];
    }
    
    public void Upgrade(Upgrade upgrade)
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif
        if (!upgrades.Contains(upgrade) || !upgrade.CanUpgrade())
        {
            return;
        }
        
        
        upgrade.ApplyUpgrade();

    }

}
