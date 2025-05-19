using UnityEngine;
using Data;
using UnityEngine.UI;

public class UIRecyclerInputSlot : MonoBehaviour
{
    [SerializeField] private ItemInstance currentItem;
    [SerializeField] private ItemData data;
    [SerializeField] private Image image;

    public ItemInstance CurrentItem
    {
        get { return currentItem; }
        set { currentItem = value; }
    }

    public ItemData Data
    {
        get { return data; }
        set { data = value; }
    }

    public void Start()
    {
        image.enabled = false;
    }

    public void Setup(ItemInstance item)
    {
        this.currentItem = item;
        this.data = item.ItemData;
        this.image.sprite = item.ItemData.sprite;
        this.image.enabled = true;
    }

    public void ResetSlot()
    {
        this.data = null;
        this.image.enabled = false;
        this.image.sprite = null;
    }

    public void RemoveItem()
    {
        if(data != null)
        {
            UIRecyclerController.Instance.RemoveItem(this);
            ResetSlot();
        }
        else
        {
            SoundManager.Instance.PlaySFX("Error");
        }
    }
}
