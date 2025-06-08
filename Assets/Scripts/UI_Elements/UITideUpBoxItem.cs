using Data;
using UnityEngine;
using UnityEngine.UI;

public class UITideUpBoxItem : MonoBehaviour
{
    [SerializeField] private Image image;
    private TrashItemInstance _trashItem;
    private TideUpBox _sourceBox;

    public void Setup(TrashItemInstance trashItem, TideUpBox box)
    {
        _trashItem = trashItem;
        _sourceBox = box;
        image.sprite = trashItem.ItemData.sprite;
    }

    public void OnClick()
    {
        _sourceBox.CollectOneItem(_trashItem);
        UITideUpBoxManager.Instance.UpdateTideUpBox();

        SoundManager.Instance.PlaySFX("Click");
    }
}
