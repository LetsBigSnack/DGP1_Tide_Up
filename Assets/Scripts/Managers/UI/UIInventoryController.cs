using UnityEngine;
using Assets.Scripts.Data;

public class UIInventoryController : UIJournalSubMenu
{
    public static UIInventoryController Instance;

    [SerializeField] private GameObject inventoryPage;
    [SerializeField] private GameObject descriptionPage;
    [SerializeField] private GameObject wallet;

    [SerializeField] private GameObject controllerFallbackItem;

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
    public override void CloseMenu()
    {
        CloseInventory();
    }

    public override void OpenMenu()
    {
        if (IsReUpCyclerOpen() || UITideUpBoxManager.Instance.IsOpen)
        {
            OpenSinglePageInventory();
            return;
        }
        OpenWholeInventory();
    }

    public void OpenWallet()
    {
        wallet.SetActive(true);
    }

    public void CloseWallet()
    {
        wallet.SetActive(false);
    }

    private bool IsReUpCyclerOpen()
    {
        ReUpcyclerType curState = UIReUpcycleManager.Instance.GetCurrentState();
        return curState == ReUpcyclerType.Recycler || curState == ReUpcyclerType.Upcycler;
    }

    public void OpenWholeInventory()
    {
        if(!inventoryPage.activeInHierarchy && !descriptionPage.activeInHierarchy)
        {
            inventoryPage.SetActive(true);
            descriptionPage.SetActive(true);
            OpenWallet();
        }
    }

    public void OpenSinglePageInventory()
    {
        if (!inventoryPage.activeInHierarchy)
        {
            UIJournalManager.Instance.State = JournalType.Inventory;
            inventoryPage.SetActive(true);
            OpenWallet();
        }
    }

    public GameObject GetFallBackButton()
    {
        return controllerFallbackItem;
    }
    public void CloseInventory()
    {
        if (inventoryPage.activeInHierarchy)
        {
            inventoryPage.SetActive(false);
        }

        if (descriptionPage.activeInHierarchy)
        {
            descriptionPage.SetActive(false);
        }

        CloseWallet();
    }
}
