using System;
using System.Collections.Generic;
using UnityEngine;


public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private Upgrade upgrade;

    public static UpgradeManager Instance;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Upgrade()
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif
        if (!upgrade.CanUpgrade())
        {
            return;
        }

        upgrade.PayUpgradeCost();
        upgrade.ApplyUpgrade();

    }

}
