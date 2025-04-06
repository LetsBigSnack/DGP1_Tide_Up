using UnityEngine;
using TMPro;

public class TestTextToSpeechInput : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    public void ReadText()
    {
        TextToSpeechManager.Instance.TranslateTextToAudio(inputField.text);
    }
}
