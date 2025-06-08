using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIToolbarItem : MonoBehaviour
{
    [SerializeField] private DeviceType type;
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

    public void UpdateCurrentItem(DeviceType device)
    {
        SetupButton(image.sprite, labelText.text, keyText.text, device);
    }

    public void SetupButton(Sprite image = null, string label = null, string key = null, DeviceType keyType = DeviceType.Keyboard)
    {
        if (label == "Move")
        {
            label = PlayerController.Instance.IsSprinting()? "Walk" : "Run";
        }

        if (keyType == DeviceType.Mouse)
        {
            keyType = DeviceType.Keyboard;
        }

        type = keyType;

        switch (keyType)
        {
            case DeviceType.Keyboard:
                this.image.sprite = image;
                this.keyText.text = key;
                this.labelText.text = label;
                break;
            case DeviceType.Mouse:
                this.image.sprite = image;
                this.keyText.text = key;
                this.labelText.text = label;
                break;
            case DeviceType.Gamepad:
                this.image.sprite = image;
                this.keyText.text = "";
                this.labelText.text = label;
                break;
            case DeviceType.Xbox:
                this.image.sprite = image;
                this.keyText.text = "";
                this.labelText.text = label;
                break;
            case DeviceType.PlayStation:
                this.image.sprite = image;
                this.keyText.text = "";
                this.labelText.text = label;
                break;
        }
    }
}
