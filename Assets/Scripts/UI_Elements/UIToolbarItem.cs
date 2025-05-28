using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIToolbarItem : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private TextMeshProUGUI labelText;

    private void OnEnable()
    {
        PlayerController.OnToggleSprintChanged += UpdateKey;
    }

    private void OnDisable()
    {
        PlayerController.OnToggleSprintChanged -= UpdateKey;
    }

    private void UpdateKey(string text)
    {
        if (TextContains("Run") || TextContains("Walk") || TextContains("Move"))
        {
            labelText.text = text;
        }
    }

    private bool TextContains(string text)
    {
        return labelText.text.Contains(text);
    }

    public void SetupButton(Sprite image = null, string label = null, string key = null, KeyType keyType = KeyType.Keyboard)
    {
        switch (keyType)
        {
            case KeyType.Keyboard:
                this.image.enabled = false;
                this.keyText.text = key;
                this.labelText.text = label;
                break;
            case KeyType.Xbox:
                this.image.enabled = true;
                this.image.sprite = image;
                this.keyText.text = "";
                this.labelText.text = label;
                break;
            case KeyType.Playstation:
                this.image.enabled = true;
                this.image.sprite = image;
                this.keyText.text = "";
                this.labelText.text = label;
                break;
        }
    }
}
