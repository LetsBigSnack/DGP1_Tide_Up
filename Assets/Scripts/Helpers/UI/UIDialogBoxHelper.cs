using UnityEngine;

public class UIDialogBoxHelper : MonoBehaviour
{
    [SerializeField] private GameObject acceptButton;
    [SerializeField] private GameObject declineButton;

    public static UIDialogBoxHelper Instance;

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

    public void Setup()
    {
        NpcDialogueManager.Instance.CurrentChoice = DialogueChoice.Accept;
        acceptButton.GetComponent<UIDialogBoxButton>().HandelHover(NpcDialogueManager.Instance.CurrentChoice);
        if(TutorialManager.Instance == null)
        {
            declineButton.SetActive(true);
            declineButton.GetComponent<UIDialogBoxButton>().HandelHover(NpcDialogueManager.Instance.CurrentChoice);
        } 
        else
        {
            declineButton.SetActive(false);
        }

    }
}

