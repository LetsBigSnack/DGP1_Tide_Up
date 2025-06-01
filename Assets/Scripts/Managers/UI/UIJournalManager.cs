using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Data;
using System;

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
    public static event Action<JournalType> OnJournalStateChanged;

    [Header("Backdrop")]
    [SerializeField] private GameObject cover;
    [SerializeField] private GameObject singleCover;
    [SerializeField] private GameObject pages;
    [SerializeField] private GameObject singlePages;

    [Header("Time Change")]
    [SerializeField] private GameObject timeChangePen;

    [Header("Journal")]
    [SerializeField] private List<UIJournalSubMenu> journalSubMenues;
    private bool _wasJournalOpen = false;

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
        KMS_Test.Instance.ClearSHIT();
        if(currentOpenType == state)
        {
            return;
        }

        currentOpenType = state;

        if (state == JournalType.Closed)
        {
            UIBookMarkController.Instance.CloseMenu();
        }
        CloseAllMenues();
        OpenMenuByType(state);
        OnJournalStateChanged?.Invoke(state);
        UIHUDManager.Instance.UpdateToolBar(GameStateManager.Instance.GetGameState());
    }

    public void CloseAllMenues()
    {
        if (_wasJournalOpen && currentOpenType == JournalType.Closed)
        {
            SoundManager.Instance.PlaySFX("Menu_close");
            _wasJournalOpen = false;
        }

        foreach (UIJournalSubMenu menu in journalSubMenues)
        {
            menu.CloseMenu();
        }
        singleCover.SetActive(false);
        singlePages.SetActive(false);
        cover.SetActive(false);
        pages.SetActive(false);
        timeChangePen.SetActive(false);

    }

   public void OpenMenuByType(JournalType type)
   {
        if(type == JournalType.Closed || TutorialManager.Instance != null && UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed)
        {
            return;
        }
        UIBookMarkController.Instance.OpenMenu();

        journalSubMenues.Where(m => m.GetComponent<UIJournalSubMenu>().JournalType == type).FirstOrDefault().OpenMenu();
        if (UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed && UIShopManager.Instance.GetCurrentState() == ShopType.Closed && !UITideUpBoxManager.Instance.IsOpen)
        {
            cover.SetActive(true);
            pages.SetActive(true);
            timeChangePen.SetActive(true);

            _wasJournalOpen = true;
            SoundManager.Instance.PlaySFX("Menu_open");
            return;
        }
        singleCover.SetActive(true);
        singlePages.SetActive(true);

        _wasJournalOpen = true;
        SoundManager.Instance.PlaySFX("Menu_open");
    }
}
