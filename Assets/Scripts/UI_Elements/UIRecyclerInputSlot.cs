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
        _button = gameObject.GetComponentInChildren<Button>();
        _button.interactable = false;
        Navigation nav = _button.navigation;
        nav.mode = Navigation.Mode.None;
        _button.navigation = nav;
    }

    public void Setup(ItemInstance item)
    {
        this.currentItem = item;
        this.data = item.ItemData;
        this.image.sprite = item.ItemData.sprite;
        this.image.enabled = true;
        _button.interactable = true;
        Navigation nav = _button.navigation;
        nav.mode = Navigation.Mode.Automatic;
        _button.navigation = nav;
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
        _button.interactable = false;
        Navigation nav = _button.navigation;
        nav.mode = Navigation.Mode.None;
        _button.navigation = nav;
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
