using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopRequirementItem : MonoBehaviour
{
    [SerializeField] private UpgradeCost currentCost;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI amount;

    public UpgradeCost GetCurrentCost()
    {
        return currentCost;
    }

    public void Setup(UpgradeCost cost, bool isCollected)
    {
        currentCost = cost;
        image.sprite = cost.isQuestItem ? cost.itemData.sprite : cost.material.sprite;
        amount.text = cost.amount.ToString();

        if (!isCollected)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
        }
        else
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        }
    }
}
