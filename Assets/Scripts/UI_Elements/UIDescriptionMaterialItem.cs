using UnityEngine;
using UnityEngine.UI;

public class UIDescriptionMaterialItem : MonoBehaviour
{
    [SerializeField] private Image image;
    
    public void Setup(Sprite sprite)
    {
        this.image.sprite = sprite;
    }
}
