using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMapLegendItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI itemName;

    public void Setup(MapData data)
    {
        this.image.sprite = data.Sprite;
        this.itemName.text = data.Title;
    }
}
