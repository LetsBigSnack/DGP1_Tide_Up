using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Data;

public enum JournalType
{
    Closed,
    Inventory,
    Map,
    FriendBook,
    Recipies,
    Calender,
    Quests
}

public class UIJournalManager : MonoBehaviour
{
    public static UIJournalManager Instance;

    [Header("CurrentState")]
    [SerializeField] private JournalType currentOpenType;

    [Header("BookMarks")]
    [SerializeField] private GameObject bookMarks;

    [Header("Backdrop")]
    [SerializeField] private GameObject cover;
    [SerializeField] private GameObject pages;

    [Header("Journal")]
    [SerializeField] private List<UIJournalSubMenu> journalSubMenues;

    public JournalType State
    {
        get { return currentOpenType; }
        set { currentOpenType = value; }
    }

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

        currentOpenType = JournalType.Closed;
    }

    public JournalType GetCurrentState()
    {
        return currentOpenType;
    }

    public void SwitchState(JournalType state)
    {
        if(state == JournalType.Closed)
        {
            UIBookMarkController.Instance.Close();
            CloseAllMenues();
            return;
        }

        CloseAllMenues();
        currentOpenType = state;
        OpenMenuByType(state);
    }

    public void CloseAllMenues()
   {
       foreach(UIJournalSubMenu menu in journalSubMenues)
       {
            menu.CloseMenu();
       }
        currentOpenType = JournalType.Closed;
        cover.SetActive(false);
        pages.SetActive(false);
    }

   public void OpenMenuByType(JournalType type)
   {
        journalSubMenues.Where(m => m.GetComponent<UIJournalSubMenu>().JournalType == type).FirstOrDefault().OpenMenu();
        UIBookMarkController.Instance.Open();
        if (UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed && UIShopManager.Instance.GetCurrentState() == ShopType.Closed)
        {
            cover.SetActive(true);
            pages.SetActive(true);
        }
   }

}
