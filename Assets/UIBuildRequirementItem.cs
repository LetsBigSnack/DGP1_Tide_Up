using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildRequirementItem : MonoBehaviour
{
    [SerializeField] private TrashMaterialEntry currentCost;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI amount;

    public TrashMaterialEntry GetCurrentCost()
    {
        return currentCost;
    }

    public void Setup(TrashMaterialEntry cost, bool isCollected)
    {
        currentCost = cost;
        image.sprite = cost.TrashMaterialData.sprite;
        amount.text = cost.Amount.ToString();

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
