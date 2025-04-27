using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : MonoBehaviour
{
    [SerializeField] private Image stateImage;
    [SerializeField] private TextMeshProUGUI questTitle;
    [SerializeField] private GameObject subQuestItemPrefab;
    [SerializeField] private Transform subQuestParent;
    [SerializeField] private Image buttonColour;

    private List<GameObject> _subQuestItems;
    private Quest _quest;

    public void OnClick()
    {
        UIQuestDescriptionHelper.Instance.UpdateQuestDetails(_quest);
    }
    public void Setup(Quest quest)
    {
        _quest = quest;

        questTitle.text = quest.QuestTitle;
        if(quest.QuestState == QuestState.Completed )
        {
            buttonColour.color = Color.gray;
        }
        Debug.Log("Done with quest Setup");
    }
}
