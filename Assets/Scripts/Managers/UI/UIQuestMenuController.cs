using Assets.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestMenuController : UIJournalSubMenu
{
    [Header("SubMenu")]
    [SerializeField] private GameObject subMenu;

    [Header("UIQuestItems")]
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] private Transform questListParent;

    private List<GameObject> _currentQuestEntries = new List<GameObject>();

    public static UIQuestMenuController Instance;
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
        subMenu.SetActive(false);
    }

    public override void OpenMenu()
    {
        subMenu.SetActive(true);
    }
}
