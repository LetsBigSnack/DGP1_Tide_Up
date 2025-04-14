using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIToolbarItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    public void SetupButton(Sprite image, string text, string key)
    {
        this.image.sprite = image;
        this.buttonText.text = text;
        this.keyText.text = key;
    }
}
