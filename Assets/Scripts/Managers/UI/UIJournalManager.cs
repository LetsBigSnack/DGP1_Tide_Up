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
        CloseAllMenues();
        OpenMenuByType(state);
        currentOpenType = state;
    }

    public void CloseAllMenues()
   {
       foreach(UIJournalSubMenu menu in journalSubMenues)
       {
            menu.CloseMenu();
       }
        currentOpenType = JournalType.Closed;
        bookMarks.SetActive(false);
    }

   public void OpenMenuByType(JournalType type)
   {
        journalSubMenues.Where(m => m.GetComponent<UIJournalSubMenu>().JournalType == type).FirstOrDefault().OpenMenu();
        bookMarks.SetActive(true);
   }

}
