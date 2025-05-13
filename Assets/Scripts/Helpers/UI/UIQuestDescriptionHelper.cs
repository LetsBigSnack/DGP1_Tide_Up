using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestDescriptionHelper : MonoBehaviour
{
    [SerializeField] private Image questNpcIcon;
    [SerializeField] private TextMeshProUGUI questTitle;
    [SerializeField] private TextMeshProUGUI questLocation;
    [SerializeField] private TextMeshProUGUI questObjectives;
    [SerializeField] private TextMeshProUGUI questReason;
    [SerializeField] private TextMeshProUGUI questNeeds;

    private UIQuestItem _currentSelectedItem;

    public static UIQuestDescriptionHelper Instance;
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

    public void SetGameObjectAsSelected(UIQuestItem item)
    {
        if (_currentSelectedItem == item)
        {
            return;
        }

        if (_currentSelectedItem == null)
        {
            _currentSelectedItem = item;
            item.ToggleIcon();
            return;
        }

        _currentSelectedItem.ToggleIcon();
        _currentSelectedItem = item;
        _currentSelectedItem.ToggleIcon();
    }

    public void UpdateQuestDetails(Quest quest)
    {
        if(quest == null)
        {
            questTitle.text = "";
            questLocation.text = "";
            questReason.text = "";
            questNeeds.text = "";
            questObjectives.text = "";
            questNpcIcon.sprite = null;

            return;
        }

        NpcData questNpc = NpcManager.Instance.GetNpcByName(quest.QuestNpc);

        questNpcIcon.sprite = questNpc.Portrait;
        questTitle.text = quest.QuestTitle;
        questLocation.text = quest.QuestLocation;
        //questObjectives.text = "??";

        if (quest.Dialogues.TryGetValue(QuestState.Offer, out Dialogue offerDialogue) && offerDialogue != null)
        {
            if (offerDialogue.DialogueContent != null && offerDialogue.DialogueContent.Count > 0)
            {
                questReason.text = offerDialogue.DialogueContent[0];
                questNeeds.text = offerDialogue.DialogueContent[1];
            }
        }

    }
}
