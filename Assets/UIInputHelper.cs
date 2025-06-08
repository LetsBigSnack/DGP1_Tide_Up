using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class UIInputHelper : MonoBehaviour
{
    private InputSystemUIInputModule _uiModule;

    private void Awake()
    {
        _uiModule = FindObjectOfType<InputSystemUIInputModule>();

        if (_uiModule == null)
        {
            Debug.LogError("UIInputNotifier requires an InputSystemUIInputModule on the same GameObject.");
        }
    }

    private void OnEnable()
    {
        if (_uiModule.move != null) _uiModule.move.action.performed += OnUINavigated;
        if (_uiModule.submit != null) _uiModule.submit.action.performed += OnUISubmitted;
        if (_uiModule.cancel != null) _uiModule.cancel.action.performed += OnUICanceled;
        if (_uiModule.point != null) _uiModule.point.action.performed += OnUIPointed;
        if (_uiModule.leftClick != null) _uiModule.leftClick.action.performed += OnUIClicked;
    }

    private void OnDisable()
    {
        if (_uiModule.move != null) _uiModule.move.action.performed -= OnUINavigated;
        if (_uiModule.submit != null) _uiModule.submit.action.performed -= OnUISubmitted;
        if (_uiModule.cancel != null) _uiModule.cancel.action.performed -= OnUICanceled;
        if (_uiModule.point != null) _uiModule.point.action.performed -= OnUIPointed;
        if (_uiModule.leftClick != null) _uiModule.leftClick.action.performed -= OnUIClicked;
    }

    private void OnUINavigated(InputAction.CallbackContext ctx)
    {
        InputDeviceHelper.Instance?.NotifyDevice(ctx.control.device, ctx.control);
    }

    private void OnUISubmitted(InputAction.CallbackContext ctx)
    {
        InputDeviceHelper.Instance?.NotifyDevice(ctx.control.device, ctx.control);
    }

    private void OnUICanceled(InputAction.CallbackContext ctx)
    {
        InputDeviceHelper.Instance?.NotifyDevice(ctx.control.device, ctx.control);
    }

    private void OnUIPointed(InputAction.CallbackContext ctx)
    {
        InputDeviceHelper.Instance?.NotifyDevice(ctx.control.device, ctx.control);
    }

    private void OnUIClicked(InputAction.CallbackContext ctx)
    {
        InputDeviceHelper.Instance?.NotifyDevice(ctx.control.device, ctx.control);
    }
}
