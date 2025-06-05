using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReUpCyclerTabSpriteHelper : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite unSelected;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color normal;
    [SerializeField] private Color highlight;

    public void OnSelected()
    {
        background.sprite = selected;
        if (text)
        {
            text.color = highlight;
        }

    }

    public void OffSelected()
    {
        background.sprite = unSelected;
        if (text)
        {
            text.color = normal;
        }
    }

}