using UnityEngine;
using UnityEngine.UI;

public class UIRecycleAllButton : MonoBehaviour
{
    [SerializeField] private Image img;

    [SerializeField] private Sprite keyboard;
    [SerializeField] private Sprite playstation;
    [SerializeField] private Sprite xbox;

    private void OnEnable()
    {
        InputDeviceHelper.OnDeviceChange += UpdateRepresentation;
    }

    private void OnDisable()
    {
        InputDeviceHelper.OnDeviceChange -= UpdateRepresentation;
    }
    
    private void UpdateRepresentation(DeviceType type)
    {
        if(type == DeviceType.Mouse)
        {
            type = DeviceType.Keyboard;
        }

        switch (type)
        {
            case DeviceType.Keyboard:
                img.sprite = keyboard;
                break;
            case DeviceType.Xbox:
                img.sprite = xbox;
                break;
            case DeviceType.PlayStation:
                img.sprite = playstation;
                break;
        }
    }
}
