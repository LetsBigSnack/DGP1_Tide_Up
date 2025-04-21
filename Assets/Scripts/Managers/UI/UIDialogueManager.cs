using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIDialogueManager : MonoBehaviour
{
    public static UIDialogueManager Instance;
    
    
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject choiceBox;
    [SerializeField] private TextMeshProUGUI nameText;
    [FormerlySerializedAs("dialoguwText")] [SerializeField] private TextMeshProUGUI dialogueText;


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

    public void ShowDialogueBox(bool show)
    {
        dialogueBox.SetActive(show);
    }

    public void SetDialogueBox(string name, string text)
    {
        ShowDialogueBox(true);
        nameText.text = name;
        dialogueText.text = text;
    }


    public void ShowChoices(bool show)
    {
        choiceBox.SetActive(show);
    }

    public void AcceptQuest()
    {
        NpcDialogueManager.Instance.MakeChoice(DialogueChoice.Accept);
    }

    public void DeclineQuest()
    {
        NpcDialogueManager.Instance.MakeChoice(DialogueChoice.Decline);
    }
    
}
