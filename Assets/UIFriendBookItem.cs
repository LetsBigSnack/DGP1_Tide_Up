using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendBookItem : MonoBehaviour
{
    [SerializeField] private NpcData currentNpc;

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI residenceText;

    [SerializeField] private Sprite emptyImage;
    [SerializeField] private string emptyTitleText;
    [SerializeField] private string emptyNameText;
    [SerializeField] private string emptyResidenceText;

    public void Setup(NpcData npc)
    {
        if(npc == null)
        {
            return;
        }

        currentNpc = npc;

        if(npc.NpcState != NpcStates.Intro)
        {
            this.image.sprite = npc.Portrait;
            this.titleText.text = npc.Title;
            this.nameText.text = npc.NpcName;
            this.residenceText.text = npc.HomeDetails;
            return;
        }

        this.image.sprite = emptyImage;
        this.titleText.text = emptyTitleText;
        this.nameText.text = emptyNameText;
        this.residenceText.text = emptyResidenceText;
    }

    public void OnClick()
    {
        if(currentNpc.NpcState != NpcStates.Intro)
        {
            UIFriendBookDescriptionHelper.Instance.Setup(currentNpc);
        }
    }
}
