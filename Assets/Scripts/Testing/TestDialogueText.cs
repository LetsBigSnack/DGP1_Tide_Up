using UnityEngine;
using TMPro;

public class TestDialogueText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogue;
    [SerializeField] private char clearTextChar;
    private string currentText = "";

    private void OnEnable()
    {
        TextToSpeechManager.OnTranslateLetterValueChanged += UpdateDialogueText;
    }

    private void OnDisable()
    {
        TextToSpeechManager.OnTranslateLetterValueChanged -= UpdateDialogueText;
    }

    private void UpdateDialogueText(char text)
    {
        if (text == clearTextChar)
        {
            currentText = "";
            dialogue.text = currentText;
            return;
        }

        if (TextToSpeechManager.Instance.GetEmotionDictionary().ContainsKey(text))
        {
            return;
        }
        currentText += text;
        dialogue.text = currentText;
    }

}
