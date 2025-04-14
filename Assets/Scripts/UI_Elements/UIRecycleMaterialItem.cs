using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRecycleMaterialItem : MonoBehaviour
{
    [SerializeField] private TrashMaterialType type;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI amount;
    [SerializeField] private int currentAmount;

    public TrashMaterialType Type
    {
        get { return type; }
        set { type = value; }
    }

    public int CurrentAmount
    {
        get { return currentAmount; }
        set { currentAmount = value; }
    }

    public void Setup(TrashMaterialType type, Sprite sprite, int amount)
    {
        this.type = type;
        this.image.sprite = sprite;
        this.currentAmount = amount;
        this.amount.text = currentAmount.ToString();
    }

    public void ChangeAmount(int amount)
    {
        currentAmount += amount;
        this.amount.text = currentAmount.ToString();
    }
}
