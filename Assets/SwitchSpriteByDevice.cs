using UnityEngine;
using UnityEngine.UI;

public class SwitchSpriteByDevice : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private Sprite keyboardSprite;
    [SerializeField] private Sprite xboxSprite;
    [SerializeField] private Sprite playstationSprite;
    [SerializeField] private Sprite controllerSprite;

    private void OnEnable()
    {
        UpdateSprite(InputDeviceHelper.Instance.GetLastDeviceType());
        InputDeviceHelper.OnDeviceChange += UpdateSprite;
    }

    private void OnDisable()
    {
        InputDeviceHelper.OnDeviceChange -= UpdateSprite;
    }

    private void UpdateSprite(DeviceType type)
    {
        if(type == DeviceType.Mouse)
        {
            type = DeviceType.Keyboard;
        }

        switch (type)
        {
            case DeviceType.Keyboard:
                img.sprite = keyboardSprite;
                break;
            case DeviceType.Xbox:
                img.sprite = xboxSprite;
                break;
            case DeviceType.PlayStation:
                img.sprite = playstationSprite;
                break;
            case DeviceType.Gamepad:
                img.sprite = controllerSprite;
                break;

        }
    }
}
