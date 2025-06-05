using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonStateByDeviceType : MonoBehaviour
{
    private Button _button;
    private void Start()
    {
        _button = gameObject.GetComponent<Button>();
    }

    private void OnEnable()
    {
        UpdateInteractability(InputDeviceHelper.Instance.GetLastDeviceType());
        InputDeviceHelper.OnDeviceChange += UpdateInteractability;        
    }

    private void OnDisable()
    {
        InputDeviceHelper.OnDeviceChange -= UpdateInteractability;
    }

    private void UpdateInteractability(DeviceType type)
    {
        StartCoroutine(UpdateInteractibilityFrameSkip(type));
    }

    private IEnumerator UpdateInteractibilityFrameSkip(DeviceType type)
    {
        yield return null;

        Navigation nav = _button.navigation;

        if (type == DeviceType.Keyboard || type == DeviceType.Mouse)
        {
            _button.interactable = true;
            nav.mode = Navigation.Mode.Automatic;
            _button.navigation = nav;
        }
        else
        {
            _button.interactable = false;
            nav.mode = Navigation.Mode.None;
            _button.navigation = nav;
        }
    }
}
