using UnityEngine;
using Data;
using UnityEngine.UI;

public class UIRecyclerInputSlot : MonoBehaviour
{
    [SerializeField] private ItemInstance currentItem;
    [SerializeField] private ItemData data;
    [SerializeField] private Image image;
    [SerializeField] private UIRecyclerInputSlot adjecent;
    private Button _button;
    private UIReUpCyclerSpriteHelper _btnHelper;

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
        _btnHelper = gameObject.GetComponent<UIReUpCyclerSpriteHelper>();
        _button = gameObject.GetComponentInChildren<Button>();
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
        if (!_button)
        {
            _button = gameObject.GetComponentInChildren<Button>();
        }
        _btnHelper?.OffSelected();
    }

    private void SwitchToAdjecentButton()
    {
        if (adjecent.gameObject.GetComponentInChildren<Button>().interactable)
        {
            return;
        }
        else
        {
            UIEventSystemHelper.Instance.SetFirstSelectedItem(adjecent.gameObject);
        }
    }

    public void RemoveItem()
    {
        if(data != null)
        {
            SwitchToAdjecentButton();
            UIRecyclerController.Instance.RemoveItem(this);
            ResetSlot();
        }
        else
        {
            SoundManager.Instance.PlaySFX("Error");
        }
    }
}
