using System.Collections.Generic;
using UnityEngine;
using Data;
using System.Linq;

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
        CloseAllMenues();
        OpenMenuByType(state);
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        currentOpenType = state;
        tabs.SetActive(true);
    }

    public void SwitchStateByInt(int i)
    {
        SwitchState((ShopType)i);
    }

    public void CloseAllMenues()
    {
        foreach (UIShopSubMenu menu in shopSubMenues)
        {
            menu.CloseMenu();
        }
        tabs.SetActive(false);
        currentOpenType = ShopType.Closed;
        GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
    }

    public void OpenMenuByType(ShopType type)
    {
        shopSubMenues.Where(m => m.GetComponent<UIShopSubMenu>().ShopType == type).FirstOrDefault().OpenMenu();
    }

}
