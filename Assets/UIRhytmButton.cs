using UnityEngine;
using UnityEngine.UI;



public class UIRhytmButton : MonoBehaviour
{
    [Header("currentType")]

    [Header("Image")]
    [SerializeField] private Image img;

    [Header("Slider")]
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject parent;

    [Header("Xbox")]
    [SerializeField] private Sprite btn_Y;
    [SerializeField] private Sprite btn_X;
    [SerializeField] private Sprite btn_A;
    [SerializeField] private Sprite btn_B;

    [Header("Playstation")]
    [SerializeField] private Sprite btnTriangle;
    [SerializeField] private Sprite btnSquare;
    [SerializeField] private Sprite btnCircle;
    [SerializeField] private Sprite btnCross;

    [Header("Keyboard")]
    [SerializeField] private Sprite btnW;
    [SerializeField] private Sprite btnA;
    [SerializeField] private Sprite btnS;
    [SerializeField] private Sprite btnD;

    private void OnEnable()
    {
        UpdateRepresentation(InputDeviceHelper.Instance.GetLastDeviceType());
        InputDeviceHelper.OnDeviceChange += UpdateRepresentation;
    }

    private void OnDisable()
    {
        InputDeviceHelper.OnDeviceChange -= UpdateRepresentation;
    }

    public void Setup(ButtonInputType buttonInputType)
    {

    }
    
    public void UpdateRepresentation(DeviceType type)
    {
        if(type == DeviceType.Mouse)
        {
            type = DeviceType.Keyboard;
        }

        switch (type)
        {
            case DeviceType.Keyboard:
                break;
            case DeviceType.PlayStation:
                break;
            case DeviceType.Xbox:
                break;
        }
    }
}
