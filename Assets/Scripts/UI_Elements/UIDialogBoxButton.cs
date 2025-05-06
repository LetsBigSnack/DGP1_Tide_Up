using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogBoxButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Sprite onHoverButton;
    [SerializeField] private Sprite offHoverButton;
    [SerializeField] private DialogueChoice btnChoice;

    private void OnEnable()
    {
        NpcDialogueManager.Instance.OnChoiceChanged += HandelHover;
    }

    private void OnDisable()
    {
        NpcDialogueManager.Instance.OnChoiceChanged -= HandelHover;
    }

    public void HandelHover(DialogueChoice choice)
    {
        if(choice == btnChoice)
        {
            buttonImage.sprite = onHoverButton;
            buttonText.color = new Color(242f / 255f, 242f / 255f, 242f / 255f);
        }
        else
        {
            buttonText.color = new Color(39f / 255f, 53f / 255f, 80f / 255f);
            buttonImage.sprite = offHoverButton;
        }
    }

    public void OnHover()
    {
        NpcDialogueManager.Instance.CurrentChoice = btnChoice;
    }
    public void OffHover()
    {
        NpcDialogueManager.Instance.CurrentChoice = btnChoice;
    }
}
