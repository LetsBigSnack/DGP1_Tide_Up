using System.Collections.Generic;
using UnityEngine;
using Data;
using System.Linq;
using System;

[Serializable]
public enum ShopType
{
    Closed,
    BoatUpgrades,
    Exchange,
    PlayerUpgrades,
}
public class UIShopManager : MonoBehaviour
{
    public static UIShopManager Instance;

    [Header("CurrentState")]
    [SerializeField] private ShopType currentOpenType;

    [Header("SubMenues")]
    [SerializeField] private List<UIShopSubMenu> shopSubMenues;
    [SerializeField] private GameObject tabs;

    private Dictionary<int, ShopType> _tabJumps = new Dictionary<int, ShopType>()
    {
        {1, ShopType.PlayerUpgrades},
        {2, ShopType.Exchange },
        {3, ShopType.BoatUpgrades },
    };

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

    public ShopType GetCurrentState()
    {
        return currentOpenType;
    }

    public void SwitchState(ShopType state)
    {
        if (state == ShopType.Closed)
        {
            CloseAllMenues();
            return;
        }

        CloseAllMenues();
        OpenMenuByType(state);
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        currentOpenType = state;
        UIHUDManager.Instance.UpdateToolBar(GameStates.InMenu);
        tabs.SetActive(true);
    }

    public void SwitchStateByInt(int i)
    {
        SwitchState((ShopType)i);
    }

    public void PreviousTab()
    {
        int enumLength = System.Enum.GetValues(typeof(ShopType)).Length;

        int next = (int)currentOpenType + 1;
        if (next >= enumLength)
            next = 1;

        if ((ShopType)next == ShopType.Closed)
            next++;

        SwitchState((ShopType)next);
    }

    public void NextTab()
    {
        int prev = (int)currentOpenType - 1;
        if (prev <= 0)
            prev = System.Enum.GetValues(typeof(ShopType)).Length - 1;

        if ((ShopType)prev == ShopType.Closed)
            prev--;

        SwitchState((ShopType)prev);
    }

    public void CloseAllMenues()
    {
        foreach (UIShopSubMenu menu in shopSubMenues)
        {
            menu.CloseMenu();
        }
        tabs.SetActive(false);
        currentOpenType = ShopType.Closed;
        GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
    }

    public void OpenMenuByType(ShopType type)
    {
        shopSubMenues.Where(m => m.GetComponent<UIShopSubMenu>().ShopType == type).FirstOrDefault().OpenMenu();
    }

}
