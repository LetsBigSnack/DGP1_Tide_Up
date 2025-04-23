using UnityEngine;
using Data;
using Assets.Scripts.Data;
using System.Collections.Generic;
using System.Linq;

public class UIFriendBookController : UIJournalSubMenu
{
    public static UIFriendBookController Instance;

    [Header("SubMenu")]
    [SerializeField] private GameObject subMenu;

    [Header("UIFriendbookItems")]
    [SerializeField] private GameObject friendBookPrefab;
    [SerializeField] private Transform friendBookParent;

    private List<GameObject> _currentFriendBookEntries = new List<GameObject>();

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
        UpdateFriendBook();
    }

    private void UpdateFriendBook()
    {
        ClearEntries();
        List<Npc> npcs = NpcManager.Instance.GetNpcs().OrderByDescending(n => n.NpcState).ThenBy(n => n.IslandID).ThenBy(n => n.NpcName).ToList();
        foreach(Npc n in npcs)
        {
            if (EnvironmentManager.Instance.IsIslandUnlocked(n.IslandID))
            {
                GameObject newBookEntry = Instantiate(friendBookPrefab, friendBookParent);
                newBookEntry.GetComponent<UIFriendBookItem>().Setup(n);
                _currentFriendBookEntries.Add(newBookEntry);
            }
        }
    }

    private void ClearEntries()
    {
        if(_currentFriendBookEntries == null || _currentFriendBookEntries.Count <= 0)
        {
            return;
        }

        foreach(GameObject i in _currentFriendBookEntries)
        {
            Destroy(i);
        }
        _currentFriendBookEntries.Clear();
    }
}
