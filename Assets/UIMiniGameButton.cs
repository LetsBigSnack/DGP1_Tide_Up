using UnityEngine;
using UnityEngine.UI;

public class UIMiniGameButton : MonoBehaviour
{
    [SerializeField] private MiniGameType type;

    [SerializeField] private Sprite interact;
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;

    [SerializeField] private Sprite xBoxInteract;
    [SerializeField] private Sprite xBoxLeft;
    [SerializeField] private Sprite xBoxRight;

    [SerializeField] private Sprite playstationInteract;
    [SerializeField] private Sprite playstationLeft;
    [SerializeField] private Sprite playstationRight;

    [SerializeField] private Image interactImg;
    [SerializeField] private Image leftImg;
    [SerializeField] private Image rightImg;

    private void OnEnable()
    {
        UpdateRepresentation(InputDeviceHelper.Instance.GetLastDeviceType());
        InputDeviceHelper.OnDeviceChange += UpdateRepresentation;
    }

    private void OnDisable()
    {
        InputDeviceHelper.OnDeviceChange -= UpdateRepresentation;
    }

    private void UpdateRepresentation(DeviceType type)
    {
        switch (type)
        {
            case DeviceType.Keyboard:
                if(this.type == MiniGameType.Fishing)
                {
                    leftImg.sprite = left;
                    rightImg.sprite = right;
                    return;
                }
                interactImg.sprite = interact;
                break;
            case DeviceType.Xbox:
                if (this.type == MiniGameType.Fishing)
                {
                    leftImg.sprite = xBoxLeft;
                    rightImg.sprite = xBoxRight;
                    return;
                }
                interactImg.sprite = xBoxInteract;
                break;
            case DeviceType.PlayStation:
                if (this.type == MiniGameType.Fishing)
                {
                    leftImg.sprite = playstationLeft;
                    rightImg.sprite = playstationRight;
                    return;
                }
                interactImg.sprite = playstationInteract;
                break;
        }
    }
}
