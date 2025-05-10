using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIDialogueManager : MonoBehaviour
{
    public static UIDialogueManager Instance;
    
    
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject choiceBox;
    [SerializeField] private TextMeshProUGUI nameText;
    [FormerlySerializedAs("dialoguwText")] [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image nameBG;

    private string _currDialogueText;
    private int _currCharCount = 0;

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

    private void OnEnable()
    {
        TextToSpeechManager.OnTranslateLetterValueChanged += SpawnInLetters;
    }

    private void OnDisable()
    {
        TextToSpeechManager.OnTranslateLetterValueChanged -= SpawnInLetters;
    }

    public void ShowDialogueBox(bool show)
    {
        dialogueBox.SetActive(show);
    }

    public void SetDialogueBox(string name, string text, Color favColor)
    {
        if(text == "")
        {
            return;
        }
        ShowDialogueBox(true);
        nameText.text = name;
        nameBG.color = favColor;
        dialogueText.text = "";
        _currDialogueText = text;
    }

    private void SpawnInLetters(char character)
    {
        if(TextToSpeechManager.Instance.IsTalking == true && !TextToSpeechManager.Instance.CharIsEmotion(character))
        {
            dialogueText.text += character;
        }
    }

    public void FinishSpeaking()
    {
        TextToSpeechManager.Instance.IsTalking = false;
        TextToSpeechManager.Instance.StopTalking();

        string sentence = "";
        foreach(char c in _currDialogueText)
        {
            if (!TextToSpeechManager.Instance.CharIsEmotion(c))
            {
                sentence += c;
            }
        }

        dialogueText.text = sentence;
    }


    public void ShowChoices(bool show)
    {
        choiceBox.SetActive(show);
        if(show)
        {
            UIDialogBoxHelper.Instance.Setup();
        }
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
