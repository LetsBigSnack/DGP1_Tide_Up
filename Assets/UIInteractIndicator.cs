using Helpers.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractIndicator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI key;
    [SerializeField] private Sprite keyBoard;
    [SerializeField] private Sprite playStation;
    [SerializeField] private Sprite xBox;

    private void OnEnable()
    {
        InputDeviceHelper.OnDeviceChange += UpdateRepresentation;
    }

    private void OnDisable()
    {
        InputDeviceHelper.OnDeviceChange -= UpdateRepresentation;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateRepresentation(DeviceType type)
    {
        switch (type)
        {
            case DeviceType.Keyboard:
                this.key.text = "E";
                break;
            case DeviceType.Mouse:
                this.key.text = "E";
                break;
            case DeviceType.Xbox:
                this.image.sprite = xBox;
                this.key.text = "";
                break;
            case DeviceType.PlayStation:
                this.image.sprite = playStation;
                this.key.text = "";
                break;
        }
    }

    public void SetUpIndicator(UIInteractionRepresentation interactRepresentation, GameObject target)
    {
        Vector3 topPosition = ColliderUtil.GetTopPosition(target);
        
        Show();
        text.text = interactRepresentation.Text;
        image.sprite = interactRepresentation.Sprite;
        transform.position = topPosition + Vector3.up * (interactRepresentation.Padding);
        UpdateRepresentation(InputDeviceHelper.Instance.GetLastDeviceType());
    }    
}
