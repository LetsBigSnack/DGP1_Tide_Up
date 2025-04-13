using UnityEngine;
using UnityEngine.UI;
using Data;

public class UIInventoryItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private ItemInstance item;

    public void Setup(ItemInstance data)
    {
        item = data;
        image.sprite = data.ItemData.sprite;
    }

    public void OnClick()
    {
        Debug.Log("Button Clicked");
    }

    public void OnHover()
    {
        if(UIJournalManager.Instance.GetCurrentType() == JournalType.Inventory && UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed)
        {
            UIItemDetailsHelper.Instance.SetupDescription(item.ItemData.title, item.ItemData.description, item.ItemData.sprite, item.ItemData.Materials);
        }
    }

    public void OffHover()
    {
        UIItemDetailsHelper.Instance.ResetDescription();
    }
}
