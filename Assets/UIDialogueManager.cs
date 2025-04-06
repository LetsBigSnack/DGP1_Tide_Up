using System;
using TMPro;
using UnityEngine;

public class UIDialogueManager : MonoBehaviour
{
    public static UIDialogueManager Instance;
    
    
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject choiceBox;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialoguwText;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
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
        dialoguwText.text = text;
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
