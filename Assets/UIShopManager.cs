using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Data;
using System.Linq;

public enum ShopType
{
    Shop,
    Exchange,
    BoatConfig
}
public class UIShopManager : MonoBehaviour
{
    public static UIShopManager Instance;

    [Header("CurrentState")]
    [SerializeField] private ShopType currentOpenType;

    [Header("Recycle/Upcycle")]
    [SerializeField] private List<UIShopSubMenu> reUpcyclerSubMenues;

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

    public void SwitchState(ShopType state)
    {
        CloseAllMenues();
        OpenMenuByType(state);
        currentOpenType = state;
    }

    public void CloseAllMenues()
    {
        foreach (UIShopSubMenu menu in reUpcyclerSubMenues)
        {
            menu.CloseMenu();
        }
    }

    public void OpenMenuByType(ShopType type)
    {
        reUpcyclerSubMenues.Where(m => m.GetComponent<UIShopSubMenu>().ShopType == type).FirstOrDefault().OpenMenu();
    }

}
