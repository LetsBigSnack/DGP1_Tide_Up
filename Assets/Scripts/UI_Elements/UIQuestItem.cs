using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : MonoBehaviour
{
    [Header("Quest info")]
    [SerializeField] private TextMeshProUGUI questTitle;

    [Header("State of Quest")]
    [SerializeField] private GameObject stateImageDoing;
    [SerializeField] private GameObject stateImageDone;

    [Header("State of Quest")]
    [SerializeField] private GameObject smallTaskSelect;
    [SerializeField] private GameObject mediumTaskSelect;
    [SerializeField] private GameObject bigTaskSelect;

    [Header("Cross outs")]
    [SerializeField] private int smallTitleMax = 16;
    [SerializeField] private int mediumTitleMax = 32;
    [SerializeField] private GameObject smallQuestDone;
    [SerializeField] private GameObject mediumQuestDone;
    [SerializeField] private GameObject bigQuestDone;

    [Header("SubQuest")]
    [SerializeField] private GameObject subQuestItemPrefab;
    [SerializeField] private Transform subQuestParent;

    private List<GameObject> _subQuestItems;
    private Quest _quest;
    private bool _isSmallTitle;
    private bool _isMediumTitle;
    private bool _isBigTitle;

    public void OnClick()
    {
        UIQuestDescriptionHelper.Instance.UpdateQuestDetails(_quest);
        UIQuestDescriptionHelper.Instance.SetGameObjectAsSelected(this);

        SoundManager.Instance.PlaySFX("Click");
    }

    public void ToggleIcon()
    {
        if (_isSmallTitle)
        {
            smallTaskSelect.SetActive(!smallTaskSelect.activeInHierarchy);
            mediumTaskSelect.SetActive(false);
            bigTaskSelect.SetActive(false);
        }
        else if(_isMediumTitle)
        {
            smallTaskSelect.SetActive(false);
            mediumTaskSelect.SetActive(!mediumTaskSelect.activeInHierarchy);
            bigTaskSelect.SetActive(false);
        }
        else
        {
            smallTaskSelect.SetActive(false);
            mediumTaskSelect.SetActive(false);
            bigTaskSelect.SetActive(!bigTaskSelect.activeInHierarchy);
        }
    }

    public void Setup(Quest quest)
    {
        _quest = quest;

        char[] title = quest.QuestTitle.ToCharArray();
        TitleLength(title);

        questTitle.text = quest.QuestTitle;

        if(quest.QuestState == QuestState.Completed )
        {
            questTitle.alpha = 0.7f;
            stateImageDoing.SetActive(false);
            stateImageDone.SetActive(true);

            if(_isSmallTitle)
            {
                smallQuestDone.SetActive(true);
            }
            else if(_isMediumTitle)
            {
                mediumQuestDone.SetActive(true);
            }
            else
            {
                bigQuestDone.SetActive(true);
            }
        }
        Debug.Log("Done with quest Setup");
    }

    private void TitleLength(char[] title)
    {
        if (title.Length < smallTitleMax)
        {
            _isSmallTitle = true;
        }
        else if (title.Length < mediumTitleMax)
        {
            _isMediumTitle = true;
        }
        else
        {
            _isBigTitle = true;
        }
    }
}
