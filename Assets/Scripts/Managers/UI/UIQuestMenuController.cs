using Assets.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestMenuController : UIJournalSubMenu
{
    [Header("SubMenu")]
    [SerializeField] private GameObject subMenu;

    [Header("UIQuestItems")]
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] private Transform currentQuestListParent;
    [SerializeField] private Transform completedQuestListParent;

    private List<Quest> _currentQuestEntries = new List<Quest>();
    private List<Quest> _completedQuestEntries = new List<Quest>();

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
        _currentQuestEntries = QuestManager.Instance.CurrentQuests;
        _completedQuestEntries = QuestManager.Instance.CompletedQuests;

        subMenu.SetActive(true);
        UpdateQuestMenu();
        LayoutRebuilder.ForceRebuildLayoutImmediate(currentQuestListParent as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(completedQuestListParent as RectTransform);
    }

    private void UpdateQuestMenu()
    {
        ClearQuestMenu();
        int counter = 0;

        foreach (Quest quest in _currentQuestEntries)
        {
            if (quest == null)
            {
                continue;
            }

            Debug.Log(quest.QuestTitle);
            GameObject newQuestItem = Instantiate(questItemPrefab, currentQuestListParent);
            newQuestItem.GetComponent<UIQuestItem>().Setup(quest);
            
            if (counter == 0)
            {
                counter = 1;
                UIQuestItem questItem = newQuestItem.GetComponent<UIQuestItem>();
                UIQuestDescriptionHelper.Instance.SetGameObjectAsSelected(questItem);
                UIEventSystemHelper.Instance.SetFirstSelectedItem(newQuestItem);
            }
        }
        foreach (Quest quest in _completedQuestEntries)
        {
            if (quest == null)
            {
                continue;
            }

            GameObject newQuestItem = Instantiate(questItemPrefab, completedQuestListParent);
            newQuestItem.GetComponent<UIQuestItem>().Setup(quest);

            if (counter == 0)
            {
                counter = 1;
                UIQuestItem questItem = newQuestItem.GetComponent<UIQuestItem>();
                UIQuestDescriptionHelper.Instance.SetGameObjectAsSelected(questItem);
                UIEventSystemHelper.Instance.SetFirstSelectedItem(newQuestItem);
            }
        }
        SelectFirstQuest();
    }

    private void ClearQuestMenu()
    {
        foreach (Transform child in currentQuestListParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in completedQuestListParent)
        {
            Destroy(child.gameObject);
        }
        UIQuestDescriptionHelper.Instance.UpdateQuestDetails(null);
    }

    private void SelectFirstQuest()
    {
        Debug.Log("count of current quests: " + _currentQuestEntries.Count);
        if (_currentQuestEntries.Count > 0)
        {
            Debug.Log("entry 0 of current quests: " + _currentQuestEntries[0]);
            UIQuestDescriptionHelper.Instance.UpdateQuestDetails(_currentQuestEntries[0]);
        }
        else if (_completedQuestEntries.Count > 0)
        {
            UIQuestDescriptionHelper.Instance.UpdateQuestDetails(_completedQuestEntries[0]);
        }
    }

}
