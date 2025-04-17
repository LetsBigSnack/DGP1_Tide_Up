using UnityEngine;
using UnityEngine.UI;

public class UIUpcycleQuestItem : MonoBehaviour
{
    [SerializeField] private Image image;

    public void Setup(Sprite sprite)
    {
        this.image.sprite = sprite;
    }
}
